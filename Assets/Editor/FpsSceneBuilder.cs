using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FpsSceneBuilder
{
    [MenuItem("Tools/Vicky FPS/Create Starter Scene")]
    public static void CreateStarterScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateLighting();
        CreateGround();
        GameObject player = CreatePlayer();
        CreateTargets();
        CreateGameManagers(player);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/StarterArena.unity");
        Selection.activeGameObject = player;
    }

    private static void CreateLighting()
    {
        GameObject lightObject = new GameObject("Sun");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.52f);
    }

    private static void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Arena Floor";
        ground.transform.localScale = new Vector3(8f, 1f, 8f);

        Material floorMaterial = new Material(Shader.Find("Standard"));
        floorMaterial.name = "Arena Floor Material";
        floorMaterial.color = new Color(0.22f, 0.25f, 0.27f);
        ground.GetComponent<Renderer>().sharedMaterial = floorMaterial;

        for (int i = 0; i < 8; i++)
        {
            CreateWall(i);
        }
    }

    private static void CreateWall(int index)
    {
        bool horizontal = index < 4;
        float offset = index % 4 < 2 ? -16f : 16f;
        float position = index % 2 == 0 ? -8f : 8f;

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Arena Cover";
        wall.transform.position = horizontal
            ? new Vector3(position, 1f, offset * 0.5f)
            : new Vector3(offset * 0.5f, 1f, position);
        wall.transform.localScale = horizontal
            ? new Vector3(4f, 2f, 0.35f)
            : new Vector3(0.35f, 2f, 4f);

        Material wallMaterial = new Material(Shader.Find("Standard"));
        wallMaterial.color = new Color(0.36f, 0.39f, 0.41f);
        wall.GetComponent<Renderer>().sharedMaterial = wallMaterial;
    }

    private static GameObject CreatePlayer()
    {
        GameObject player = new GameObject("FPS Player");
        player.transform.position = new Vector3(0f, 1.1f, -8f);

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.9f, 0f);

        GameObject cameraObject = new GameObject("Player Camera");
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.55f, 0f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.nearClipPlane = 0.05f;
        camera.fieldOfView = 74f;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.tag = "MainCamera";

        FpsPlayerController playerController = player.AddComponent<FpsPlayerController>();
        playerController.cameraRoot = cameraObject.transform;

        GameObject weaponObject = new GameObject("Prototype Rifle");
        weaponObject.transform.SetParent(cameraObject.transform);
        weaponObject.transform.localPosition = new Vector3(0.35f, -0.28f, 0.55f);

        HitscanWeapon weapon = weaponObject.AddComponent<HitscanWeapon>();
        weapon.playerCamera = camera;

        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(weaponObject.transform);
        muzzle.transform.localPosition = new Vector3(0f, 0f, 0.35f);
        weapon.muzzlePoint = muzzle.transform;

        LineRenderer tracer = weaponObject.AddComponent<LineRenderer>();
        tracer.startWidth = 0.025f;
        tracer.endWidth = 0.01f;
        tracer.material = new Material(Shader.Find("Sprites/Default"));
        tracer.startColor = Color.yellow;
        tracer.endColor = Color.clear;
        weapon.tracer = tracer;

        GameObject weaponMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        weaponMesh.name = "Temporary Weapon Mesh";
        weaponMesh.transform.SetParent(weaponObject.transform);
        weaponMesh.transform.localPosition = Vector3.zero;
        weaponMesh.transform.localScale = new Vector3(0.26f, 0.16f, 0.65f);
        UnityEngine.Object.DestroyImmediate(weaponMesh.GetComponent<Collider>());

        return player;
    }

    private static void CreateTargets()
    {
        Material targetMaterial = new Material(Shader.Find("Standard"));
        targetMaterial.name = "Target Material";
        targetMaterial.color = new Color(0.9f, 0.18f, 0.12f);

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;
            Vector3 position = new Vector3(Mathf.Sin(angle) * 9f, 1f, Mathf.Cos(angle) * 9f);

            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            target.name = "Damageable Target";
            target.transform.position = position;
            target.transform.localScale = new Vector3(1f, 1.2f, 1f);
            target.GetComponent<Renderer>().sharedMaterial = targetMaterial;
            target.AddComponent<DamageableTarget>();
        }
    }

    private static void CreateGameManagers(GameObject player)
    {
        GameObject gameManager = new GameObject("Game Manager");
        gameManager.AddComponent<GameManager>();

        GameHud hud = gameManager.AddComponent<GameHud>();
        hud.weapon = player.GetComponentInChildren<HitscanWeapon>();
    }
}
