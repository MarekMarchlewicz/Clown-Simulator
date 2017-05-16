using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedController))]
public class BalloonSpawner : MonoBehaviour
{
    [SerializeField]
    private Balloon balloon;

    [SerializeField]
    private GameObject colourWheel;

    [SerializeField]
    private Transform balloonSpawnTarget;

    [SerializeField]
    private MeshRenderer colourIndicator;

    private SteamVR_TrackedController controller;

    private bool isTouching = false;

    private Vector2 padTouchPosition;

	private void Start ()
    {
        controller = GetComponent<SteamVR_TrackedController>();

        controller.TriggerClicked += OnTriggerClicked;
        controller.TriggerUnclicked += OnTriggerUnlicked;
        controller.PadTouched += OnPadTouched;
        controller.PadUntouched += OnPadUntouched;
        balloon.Initialize(balloonSpawnTarget);
	}

    private void OnPadTouched(object sender, ClickedEventArgs e)
    {
        isTouching = true;
    }

    private void OnPadUntouched(object sender, ClickedEventArgs e)
    {
        isTouching = false;
    }

    private void OnTriggerClicked(object sender, ClickedEventArgs e)
    {
        balloon.StartInflating();
    }

    private void OnTriggerUnlicked(object sender, ClickedEventArgs e)
    {
        balloon.StopInflating();
    }

    private void Update()
    {
        if (isTouching)
        {
            padTouchPosition = new Vector2(controller.controllerState.rAxis0.x, controller.controllerState.rAxis0.y);
            
            float angle = Vector2.Angle(Vector2.up, padTouchPosition) * Mathf.Sign(-padTouchPosition.x);
            if (angle < 0f) angle += 360f;
            angle /= 360f;

            float s = Mathf.Clamp01(padTouchPosition.magnitude);

            Color colour = Color.HSVToRGB(angle, s, 1f);

            colourIndicator.material.color = colour;

            balloon.SetColour(colour);
        }
    }
}
