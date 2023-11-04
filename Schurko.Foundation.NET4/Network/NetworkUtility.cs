using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Network
{
    /// <summary>
	/// Utilities for network information.
	/// </summary>
	public static class NetworkUtility
    {
        /// <summary>
        /// Returns the MAC (Media Access Control or physical address) for the first currently active (can transmit data packets) network interface (adapter) on the local computer.
        /// <para>If you want a <see cref="PhysicalAddress"/>, call <see cref="GetPhysicalAddress"/>.</para>
        /// </summary>
        public static string GetMacAddress()
        {
            return GetPhysicalAddress()?.ToString();
        }

        /// <summary>
        /// Returns the physical address (or Media Access Control (MAC)) for the first currently active (can transmit data packets) network interface (adapter) on the local computer.
        /// <para>If you want a string, call <see cref="GetMacAddress"/>.</para>
        /// </summary>
        public static PhysicalAddress GetPhysicalAddress()
        {
            return GetNetworkInterface()?.GetPhysicalAddress();
        }

        /// <summary>
        /// Returns the first currently active (can transmit data packets) network interface (adapter) on the local computer.
        /// </summary>
        public static NetworkInterface GetNetworkInterface()
        {
            return GetNetworkInterfaces().FirstOrDefault();
        }

        /// <summary>
        /// Returns the currently active (can transmit data packets) network interfaces (adapters) on the local computer.
        /// </summary>
        public static IEnumerable<NetworkInterface> GetNetworkInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
        }
    }
}
