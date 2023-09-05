using Automation.BDaq;
using System;

namespace IOControl
{
    public class PCIE
    {
        public PCIE()
        {
            instantDiCtrl1730 = new InstantDiCtrl();
            instantDoCtrl1730 = new InstantDoCtrl();
            SelectDevice(1);

            instantDiCtrl1760 = new InstantDiCtrl();
            instantDoCtrl1760 = new InstantDoCtrl();
            SelectDevice(2);
        }
        ~PCIE()
        {
            instantDiCtrl1730.Dispose();
            instantDoCtrl1730.Dispose();

            instantDiCtrl1760.Dispose();
            instantDoCtrl1760.Dispose();
        }

        InstantDiCtrl instantDiCtrl1730;
        InstantDoCtrl instantDoCtrl1730;

        InstantDiCtrl instantDiCtrl1760;
        InstantDoCtrl instantDoCtrl1760;

        public int FeaturesInPortCount { get { return instantDiCtrl1760.Features.PortCount; } }
        public int FeaturesOutPortCount { get { return instantDoCtrl1730.Features.PortCount; } }

        private bool Init1730 = false;
        private bool Init1760 = false;

        /// <summary>
        /// return the port data to update the port state
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public ErrorCode GetPortData(int port, out byte portData)
        {
            return instantDiCtrl1730.Read(port, out portData);
        }
        public ErrorCode GetOutputPortData(int port, out byte portData)
        {
            return instantDoCtrl1730.Read(port, out portData);
        }
        public ErrorCode GetPortData(int ioIndex, int port, out byte portData)
        {
            //SelectDevice(ioIndex);
            if (ioIndex == 1)
            {
                return instantDiCtrl1730.Read(port, out portData);
            }
            else
            {
                return instantDiCtrl1760.Read(port, out portData);
            }
        }

        /// <summary>
        /// write the state to the PCIe
        /// </summary>
        /// <param name="port"></param>
        /// <param name="bit"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ErrorCode WritePortData(int port, int bit, byte state)
        {
            return instantDoCtrl1730.WriteBit(port, bit, state);
        }
        public ErrorCode WritePortData(int port, byte state)
        {
            return instantDoCtrl1730.Write(port, state);
        }
        public ErrorCode WritePortData(int ioIndex, int port, int bit, byte state)
        {
            //SelectDevice(ioIndex);
            if (ioIndex == 1)
            {
                return instantDoCtrl1730.WriteBit(port, bit, state);
            }
            else if (ioIndex == 2)
            {
                return instantDoCtrl1760.WriteBit(port, bit, state);
            }
            return ErrorCode.WarningBurnout;
        }

        /// <summary>
        /// select the specified device by index
        /// </summary>
        /// <param name="index">the device index</param>
        private bool SelectDevice(int index)
        {
            try
            {
                if (index == 1 && !Init1730)
                {
                    instantDiCtrl1730.SelectedDevice = new DeviceInformation(index);
                    instantDoCtrl1730.SelectedDevice = new DeviceInformation(index);
                    Init1730 = true;
                }
                else if (index == 2 && !Init1760)
                {
                    instantDiCtrl1760.SelectedDevice = new DeviceInformation(index);
                    instantDoCtrl1760.SelectedDevice = new DeviceInformation(index);
                    Init1760 = true;
                }

                if (!instantDiCtrl1730.Initialized || !instantDoCtrl1730.Initialized)
                {
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Closes the selected device and release the resources allocated, can be reused
        /// </summary>
        public void CloseDevice()
        {
            instantDiCtrl1730.Cleanup();
            instantDiCtrl1730.Cleanup();
        }
    }
}