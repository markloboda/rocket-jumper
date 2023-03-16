using System;

namespace RocketJumper.Classes.Controls
{
    public class ReplayEventArgs : EventArgs
    {
        public ReplayEventArgs(string replayId)
        {
            ReplayId = replayId;
        }

        public string ReplayId { get; private set; }
    }
}