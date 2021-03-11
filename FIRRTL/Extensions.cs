using System;

namespace FIRRTL
{
    public static class Extensions
    {
        public static Dir Flip(this Dir dir, Orientation orien)
        {
            if (orien == Orientation.Normal)
            {
                return dir;
            }

            return dir switch
            {
                Dir.Input => Dir.Output,
                Dir.Output => Dir.Input,
                _ => throw new Exception($"Invalid dir. Dir: {dir}")
            };
        }
    }
}
