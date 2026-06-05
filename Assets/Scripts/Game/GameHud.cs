using UnityEngine;

public class GameHud : MonoBehaviour
{
    public HitscanWeapon weapon;

    private GUIStyle labelStyle;
    private GUIStyle crosshairStyle;

    private void Awake()
    {
        if (weapon == null)
        {
            weapon = FindObjectOfType<HitscanWeapon>();
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;
        GUI.Label(new Rect(centerX - 8f, centerY - 12f, 16f, 24f), "+", crosshairStyle);

        int score = GameManager.Instance != null ? GameManager.Instance.Score : 0;
        GUI.Label(new Rect(24f, 18f, 260f, 32f), $"Score: {score}", labelStyle);

        if (weapon != null)
        {
            string reloadText = weapon.IsReloading ? "  Reloading" : "";
            GUI.Label(
                new Rect(Screen.width - 260f, Screen.height - 58f, 240f, 36f),
                $"Ammo: {weapon.AmmoInMagazine}/{weapon.magazineSize}{reloadText}",
                labelStyle);
        }
    }

    private void EnsureStyles()
    {
        if (labelStyle != null)
        {
            return;
        }

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            normal = { textColor = Color.white }
        };

        crosshairStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
    }
}
