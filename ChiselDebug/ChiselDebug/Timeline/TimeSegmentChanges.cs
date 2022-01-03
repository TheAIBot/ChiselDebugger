using ChiselDebug.GraphFIR.Circuit;
using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.Timeline
{
    internal class TimeSegmentChanges
    {
        public readonly TimeSpan TimeInterval;
        private readonly CircuitState StartState;
        private readonly BinaryVarValue[] Changes;
        private readonly List<TimeStepChanges> StepChanges;

        public TimeSegmentChanges(TimeSpan interval, CircuitState startState, BinaryVarValue[] changes, List<TimeStepChanges> timeSteps)
        {
            this.TimeInterval = interval;
            this.StartState = startState;
            this.Changes = changes;
            this.StepChanges = timeSteps;
        }

        public CircuitState GetStateAtTime(ulong time)
        {
            CircuitState state = StartState.Copy();
            int length = 0;
            foreach (var step in StepChanges)
            {
                if (step.Time > time)
                {
                    break;
                }

                length = step.StartIndex;
            }

            state.AddChanges(Changes.AsSpan(0, length), time);

            return state;
        }

        public IEnumerable<CircuitState> GetAllDistinctStates()
        {
            CircuitState state = StartState.Copy();
            foreach (var step in StepChanges)
            {
                state.AddChanges(step.GetChanges(Changes), step.Time);
                yield return state;
            }
        }

        public IEnumerable<ulong> GetAllSimTimes()
        {
            foreach (var step in StepChanges)
            {
                yield return step.Time;
            }
        }
    }
}
