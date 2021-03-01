using System.Collections.Generic;

namespace ChiselDebug.Timeline
{
    internal class TimeSegmentChanges
    {
        public readonly TimeSpan TimeInterval;
        private readonly CircuitState StartState;
        private readonly List<TimeStepChanges> StepChanges = new List<TimeStepChanges>();

        public TimeSegmentChanges(TimeSpan interval, CircuitState startState, List<TimeStepChanges> changes)
        {
            this.TimeInterval = interval;
            this.StartState = startState;
            this.StepChanges = changes;
        }

        public CircuitState GetStateAtTime(ulong time)
        {
            CircuitState state = StartState.Copy();
            foreach (var step in StepChanges)
            {
                if (step.Time > time)
                {
                    break;
                }

                state.AddChanges(step);
            }

            return state;
        }
    }
}
