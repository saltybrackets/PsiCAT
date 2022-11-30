﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YeelightAPI
{
    /// <summary>
    /// Group of Yeelight Devices
    /// </summary>
    public partial class DeviceGroup : List<YeelightDevice>, IDisposable
    {
        #region PUBLIC PROPERTIES

        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; set; }

        #endregion PUBLIC PROPERTIES

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="name"></param>
        public DeviceGroup(string name = null)
        {
            Name = name;
        }

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="yeelightDevice"></param>
        /// <param name="name"></param>
        public DeviceGroup(YeelightDevice yeelightDevice, string name = null)
        {
            Add(yeelightDevice);
            Name = name;
        }

        /// <summary>
        /// Constructor with devices as params
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(params YeelightDevice[] devices)
        {
            AddRange(devices);
        }

        /// <summary>
        /// Constructor with a list (IEnumerable) of devices
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="name"></param>
        public DeviceGroup(IEnumerable<YeelightDevice> devices, string name = null)
        {
            AddRange(devices);
            Name = name;
        }

        #endregion CONSTRUCTOR

        #region IDisposable

        /// <summary>
        /// Dispose the devices
        /// </summary>
        public void Dispose()
        {
            foreach (YeelightDevice device in this)
            {
                device.Dispose();
            }
        }

        #endregion IDisposable

        #region Protected Methods

        /// <summary>
        /// Execute code for all the devices
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        protected async Task<bool> Process(Func<YeelightDevice, Task<bool>> f)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (YeelightDevice device in this)
            {
                tasks.Add(f(device));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Count} devices)";
        }

        #endregion Protected Methods
    }
}