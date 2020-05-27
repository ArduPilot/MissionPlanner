using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rtsp
{
    using Messages;
    /// <summary>
    /// Event args containing information for message events.
    /// </summary>
    public class RtspChunkEventArgs :EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspChunkEventArgs"/> class.
        /// </summary>
        /// <param name="aMessage">A message.</param>
        public RtspChunkEventArgs(RtspChunk aMessage)
        {
            Message = aMessage;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public RtspChunk Message { get; set; }
    }
}
