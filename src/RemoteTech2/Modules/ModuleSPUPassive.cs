﻿using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RemoteTech
{
    public class ModuleSPUPassive : PartModule, ISignalProcessor
    {
        public String Name { get { return String.Format("ModuleSPUPassive({0})", VesselName); } }
        public String VesselName { get { return vessel.vesselName; } set { vessel.vesselName = value; } }
        public bool VesselLoaded { get { return vessel.loaded; } }
        public Guid Guid { get { return vessel.id; } }
        public Vector3 Position { get { return vessel.GetWorldPos3D(); } }
        public CelestialBody Body { get { return vessel.mainBody; } }
        public bool Visible { get { return MapViewFiltering.CheckAgainstFilter(vessel); } }
        public bool Powered { get { return Vessel.IsControllable; } }
        public bool IsCommandStation { get { return false; } }
        public FlightComputer FlightComputer { get { return null; } }
        public Vessel Vessel { get { return vessel; } }
        public bool IsRoot { get { return Vessel.GetReferenceTransformPart() == part; } }

        private ISatellite Satellite { get { return RTCore.Instance.Satellites[mRegisteredId]; } }

        private Guid mRegisteredId;

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor)
            {
                GameEvents.onVesselWasModified.Add(OnVesselModified);
                GameEvents.onPartUndock.Add(OnPartUndock);
                mRegisteredId = RTCore.Instance.Satellites.Register(vessel, this);
            }
        }

        public void OnDestroy()
        {
            GameEvents.onVesselWasModified.Remove(OnVesselModified);
            GameEvents.onPartUndock.Remove(OnPartUndock);
            if (RTCore.Instance != null)
            {
                RTCore.Instance.Satellites.Unregister(mRegisteredId, this);
                mRegisteredId = Guid.Empty;
            }
        }

        public void OnPartUndock(Part p)
        {
            OnVesselModified(p.vessel);
        }

        public void OnVesselModified(Vessel v)
        {
            if ((mRegisteredId != vessel.id))
            {
                RTCore.Instance.Satellites.Unregister(mRegisteredId, this);
                mRegisteredId = RTCore.Instance.Satellites.Register(vessel, this);
            }
        }
    }
}