﻿using Valve.VR;

namespace VRCOSC.OpenVR.Device;

public class OVRDevice
{
    /// <summary>
    /// The OVR ID of the device
    /// </summary>
    public uint Id;

    /// <summary>
    /// Whether the device has been registed in this OVR session
    /// </summary>
    public bool IsPresent { get; private set; }

    /// <summary>
    /// Whether the device is currently connected to this OVR session
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Whether the device can provide battery information
    /// </summary>
    public bool CanProvideBatteryInfo { get; private set; }

    /// <summary>
    /// Whether the device is currently charging
    /// </summary>
    public bool IsCharging { get; private set; }

    /// <summary>
    /// The device's battery between 0 and 1
    /// </summary>
    public float BatteryPercentage { get; private set; }

    public void BindTo(uint id)
    {
        Id = id;
    }

    public void Update()
    {
        IsPresent = Id != uint.MaxValue;
        IsConnected = IsPresent && IsTrackedDeviceConnected();
        CanProvideBatteryInfo = OVRHelper.GetBoolTrackedDeviceProperty(Id, ETrackedDeviceProperty.Prop_DeviceProvidesBatteryStatus_Bool);
        IsCharging = OVRHelper.GetBoolTrackedDeviceProperty(Id, ETrackedDeviceProperty.Prop_DeviceIsCharging_Bool);
        BatteryPercentage = OVRHelper.GetFloatTrackedDeviceProperty(Id, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float);
    }

    protected virtual bool IsTrackedDeviceConnected() => Valve.VR.OpenVR.System.IsTrackedDeviceConnected(Id);
}
