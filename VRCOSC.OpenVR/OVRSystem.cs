﻿using Valve.VR;
using VRCOSC.OpenVR.Device;

namespace VRCOSC.OpenVR;

public class OVRSystem
{
    private readonly SortedDictionary<uint, OVRDevice> devices = new();

    public HMD? HMD => devices.Values.OfType<HMD>().SingleOrDefault();
    public Controller? LeftController => devices.Values.OfType<Controller>().SingleOrDefault(controller => controller.Role == ETrackedControllerRole.LeftHand);
    public Controller? RightController => devices.Values.OfType<Controller>().SingleOrDefault(controller => controller.Role == ETrackedControllerRole.RightHand);
    public IEnumerable<GenericTracker> Trackers => devices.Values.OfType<GenericTracker>();

    public void Init()
    {
        devices.Clear();

        for (uint i = 0; i < Constants.MAX_DEVICE_COUNT; i++)
        {
            RegisterDevice(i);
        }
    }

    public void RegisterDevice(uint id)
    {
        var deviceClass = Valve.VR.OpenVR.System.GetTrackedDeviceClass(id);

        switch (deviceClass)
        {
            case ETrackedDeviceClass.HMD:
                devices.Add(id, new HMD(id));
                break;

            case ETrackedDeviceClass.Controller:
                devices.Add(id, new Controller(id));
                break;

            case ETrackedDeviceClass.GenericTracker:
                devices.Add(id, new GenericTracker(id));
                break;

            case ETrackedDeviceClass.TrackingReference:
            case ETrackedDeviceClass.DisplayRedirect:
            case ETrackedDeviceClass.Max:
            case ETrackedDeviceClass.Invalid:
                break;
        }
    }

    public void UpdateDevice(uint id)
    {
        devices[id].Update();
    }
}
