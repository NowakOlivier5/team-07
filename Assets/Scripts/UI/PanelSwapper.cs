using System.Collections.Generic;
using UnityEngine;

namespace MMSteamMulti
{
    public class PanelSwapper : MonoBehaviour
    {
        public List<Panel> panels = new List<Panel>();

        public void SwapPanel(string panelName)
        {
            foreach (Panel panel in panels)
            {
                if (panel.PanelName == panelName)
                {
                    panel.gameObject.SetActive(true);
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }
            }
        }
    }
}