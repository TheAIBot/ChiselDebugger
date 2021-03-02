﻿using ChiselDebug.GraphFIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader;

namespace ChiselDebug.Timeline
{
    public class VCDTimeline
    {
        public readonly TimeScale TimeScale;
        private readonly TimeSpan TimeInterval;
        private readonly List<TimeSegmentChanges> SegmentChanges = new List<TimeSegmentChanges>();

        public VCDTimeline(VCD vcd)
        {
            this.TimeScale = vcd.Time;

            DumpVars initDump = vcd.GetSimulationCommands().Take(1).First() as DumpVars;
            CircuitState segmentStartState = new CircuitState(initDump);
            CircuitState followState = segmentStartState.Copy();

            ulong startTime = 0;
            List<TimeStepChanges> stepChanges = new List<TimeStepChanges>();
            TimeStepChanges currTimeStep = null;

            int changeCounter = 0;
            const int maxChangesPerSegment = 10_000;

            foreach (var simCmd in vcd.GetSimulationCommands())
            {
                if (simCmd is SimTime time)
                {
                    if (currTimeStep != null)
                    {
                        followState.AddChanges(currTimeStep);
                        stepChanges.Add(currTimeStep);

                        //If segment is full then store the segment and
                        //prepare for the next segment
                        if (changeCounter > maxChangesPerSegment)
                        {
                            TimeSpan tSpan = new TimeSpan(startTime, time.Time);
                            SegmentChanges.Add(new TimeSegmentChanges(tSpan, segmentStartState, stepChanges));

                            segmentStartState = followState;
                            followState = segmentStartState.Copy();

                            stepChanges = new List<TimeStepChanges>();
                            changeCounter = 0;

                            startTime = time.Time;
                        }
                    }
                    else
                    {
                        startTime = time.Time;
                    }

                    currTimeStep = new TimeStepChanges(time.Time);
                }
                else if (simCmd is VarValue change)
                {
                    currTimeStep.Changes.Add(change);
                    changeCounter++;
                }
            }

            //Add last segment if it's not empty.
            //Segment isn't added by above loop if the vcd file doesn't 
            //end with a simulation time command.
            if (stepChanges.Count > 0)
            {
                TimeSpan tSpan = new TimeSpan(startTime, stepChanges.Last().Time + 1);
                SegmentChanges.Add(new TimeSegmentChanges(tSpan, segmentStartState, stepChanges));
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
    }
}
