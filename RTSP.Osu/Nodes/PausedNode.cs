﻿using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "IsPaused")]
    class PausedNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            return await Task.FromResult(false);
        }
    }
}
