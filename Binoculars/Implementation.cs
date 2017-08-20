﻿using UnityEngine;

using ModComponentMapper;

namespace Binoculars
{
    public class Implementation
    {
        public GameObject GameObject;
        private GameObject equippedModel;

        private float originalFOV;
        private UITexture texture;

        public void OnEquipped()
        {
            InterfaceManager.QuitCurrentScreens();

            ShowEquippedModel();
            ShowButtonPopups();
        }

        public void OnUse()
        {
            GameManager.GetPlayerManagerComponent().EquipItem(this.GameObject.GetComponent<GearItem>(), false);
        }

        public void OnUnequipped()
        {
            EndZoom();
            HideEquippedModel();
        }

        public void OnControlModeChangedWhileEquipped()
        {
            EndZoom();
        }

        public void OnSecondaryAction()
        {
            if (GameManager.GetVpFPSCamera().IsZoomed)
            {
                EndZoom();
            }
            else
            {
                StartZoom();
            }
        }

        private void StartZoom()
        {
            ModUtils.FreezePlayer();
            ZoomCamera();
            ShowOverlay();
        }

        private void EndZoom()
        {
            if (!GameManager.GetVpFPSCamera().IsZoomed)
            {
                return;
            }

            ModUtils.UnfreezePlayer();
            RestoreCamera();
            HideOverlay();
        }

        private void ZoomCamera()
        {
            vp_FPSCamera camera = GameManager.GetVpFPSCamera();
            originalFOV = ModUtils.GetFieldValue<float>(camera, "m_RenderingFieldOfView");
            camera.ToggleZoom(true);
            camera.SetFOVFromOptions(originalFOV * 0.1f);
        }

        private void RestoreCamera()
        {
            var camera = GameManager.GetVpFPSCamera();
            camera.ToggleZoom(false);
            camera.SetFOVFromOptions(originalFOV);
        }

        private void ShowEquippedModel()
        {
            GameObject prefab = (GameObject)Resources.Load("TOOL_Binoculars");

            this.equippedModel = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            this.equippedModel.transform.parent = GameManager.GetVpFPSCamera().transform;
            this.equippedModel.transform.localPosition = new Vector3(0, -0.25f, 0.25f);
            this.equippedModel.transform.localRotation = Quaternion.Euler(0, -90, -60);
        }

        private void HideEquippedModel()
        {
            if (equippedModel == null)
            {
                return;
            }

            Object.Destroy(equippedModel);
            equippedModel = null;
        }

        private void ShowOverlay()
        {
            texture = UIUtils.CreateOverlay((Texture2D)Resources.Load("Binoculars_Overlay"));
        }

        private void HideOverlay()
        {
            if (texture == null)
            {
                return;
            }

            Object.Destroy(texture);
            texture = null;
        }

        private static void ShowButtonPopups()
        {
            EquipItemPopupUtils.ShowItemPopups(string.Empty, Localization.Get("GAMEPLAY_Use"), false, false, false, true);
        }
    }
}