using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Events
{
    /// <summary>
    /// Device found event argument
    /// </summary>
    public class DeviceFoundEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Notification Result
        /// </summary>
        public YeelightDevice YeelightDevice { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceFoundEventArgs() { }

        /// <summary>
        /// Constructor with notification result
        /// </summary>
        /// <param name="yeelightDevice"></param>
        public DeviceFoundEventArgs(YeelightDevice yeelightDevice)
        {
            this.YeelightDevice = yeelightDevice;
        }

        #endregion Public Constructors
    }
}
