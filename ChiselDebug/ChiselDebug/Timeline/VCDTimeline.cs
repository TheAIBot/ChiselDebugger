using ChiselDebug.GraphFIR.Circuit;
using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebug.Timeline
{
    public sealed class VCDTimeline
    {
        public readonly TimeSpan TimeInterval;
        private readonly List<TimeSegmentChanges> SegmentChanges = new List<TimeSegmentChanges>();
        public int StateCount { get; private set; } = 0;

        public VCDTimeline(VCD vcd)
        {
            var simCommands = vcd.GetSimulationCommands();
            SimPass firstCmd = simCommands.First();
            CircuitState segmentStartState;
            if (firstCmd.SimCmd is DumpVars initDump)
            {
                segmentStartState = new CircuitState(initDump);
            }
            else
            {
                segmentStartState = new CircuitState(vcd.Variables.ToList());
                simCommands = simCommands.Prepend(firstCmd);
            }
            CircuitState followState = segmentStartState.Copy();

            ulong startTime = 0;
            const int maxChangesPerSegment = 100_000;
            List<BinaryVarValue> binChanges = new List<BinaryVarValue>(maxChangesPerSegment);
            List<TimeStepChanges> stepChanges = new List<TimeStepChanges>();
            int currTimeStepStart = 0;
            int currTimeStepLength = 0;


            foreach (var simCmd in simCommands)
            {
                if (simCmd.SimCmd is SimTime time)
                {
                    if (currTimeStepLength > 0)
                    {
                        StateCount++;
                        TimeStepChanges timeStep = new TimeStepChanges(startTime, currTimeStepStart, currTimeStepLength);
                        currTimeStepStart += currTimeStepLength;
                        currTimeStepLength = 0;

                        stepChanges.Add(timeStep);

                        //If segment is full then store the segment and
                        //prepare for the next segment
                        if (binChanges.Count > maxChangesPerSegment)
                        {
                            TimeSpan tSpan = new TimeSpan(stepChanges.First().Time, time.Time);
                            SegmentChanges.Add(new TimeSegmentChanges(tSpan, segmentStartState, binChanges.ToArray(), stepChanges));
                            currTimeStepStart = 0;
                            currTimeStepLength = 0;

                            segmentStartState = followState;
                            followState = segmentStartState.Copy();

                            binChanges.Clear();
                            stepChanges = new List<TimeStepChanges>();
                        }


                    }
                    startTime = time.Time;
                }
                else if (simCmd.HasBinValue)
                {
                    followState.AddChange(simCmd.BinValue);
                    binChanges.Add(simCmd.BinValue);
                    currTimeStepLength++;
                }
            }

            //Add last segment if it's not empty.
            //Segment isn't added by above loop if the vcd file doesn't 
            //end with a simulation time command.
            if (currTimeStepLength > 0)
            {
                stepChanges.Add(new TimeStepChanges(startTime, currTimeStepStart, currTimeStepLength));
            }
            if (stepChanges.Count > 0)
            {
                TimeSpan tSpan = new TimeSpan(stepChanges.First().Time, stepChanges.Last().Time + 1);
                SegmentChanges.Add(new TimeSegmentChanges(tSpan, segmentStartState, binChanges.ToArray(), stepChanges));
            }

            this.TimeInterval = new TimeSpan(SegmentChanges.First().TimeInterval.StartInclusive, SegmentChanges.Last().TimeInterval.EndExclusive);
        }

        public CircuitState GetStateAtTime(ulong time)
        {
            if (!TimeInterval.IsTimeInTimeSpan(time))
            {
                throw new Exception($"Requested time does not exist in this timeline. Time: {time}");
            }

            TimeSegmentChanges segment = SegmentChanges.First(x => x.TimeInterval.IsTimeInTimeSpan(time));
            return segment.GetStateAtTime(time);
        }

        public IEnumerable<CircuitState> GetAllDistinctStates()
        {
            foreach (var segment in SegmentChanges)
            {
                foreach (var state in segment.GetAllDistinctStates())
                {
                    yield return state;
                }
            }
        }

        public IEnumerable<ulong> GetAllSimTimes()
        {
            foreach (var segment in SegmentChanges)
            {
                foreach (var time in segment.GetAllSimTimes())
                {
                    yield return time;
                }
            }
        }

        public CircuitState GetFirstState()
        {
            return GetStateAtTime(TimeInterval.StartInclusive);
        }
    }
}
