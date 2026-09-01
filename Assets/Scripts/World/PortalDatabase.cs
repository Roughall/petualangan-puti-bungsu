using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game Data/Portal Database")]
public class PortalDatabase : ScriptableObject
{
    public List<PortalData> portals;

        public PortalData GetPortal(PortalID id)
        {
            foreach (PortalData portal in portals)
            {
                if (portal.portalID == id)
                return portal;
            }

            Debug.LogWarning("Portal tidak ditemukan : " + id);
            return null;
        }
}