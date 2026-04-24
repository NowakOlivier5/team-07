using System.Collections.Generic;
using UnityEngine;

// A different form of handling navigation around the main menu.
// Not fully implemented but expected to replace the old version fully
// References: https://www.youtube.com/watch?v=WBOvM2pjt9E
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