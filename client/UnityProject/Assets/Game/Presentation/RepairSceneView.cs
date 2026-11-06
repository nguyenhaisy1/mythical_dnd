using System;
using Game.Application.Projection;
using UnityEngine;

namespace Game.Client.Sprint15
{
    /// <summary>All local positions are presentation data. They never enter WorldState or a gameplay command.</summary>
    public static class RepairSceneSpec
    {
        public const int CanvasWidth = 1920, CanvasHeight = 1080;
        public const float PixelsPerUnit = 120, OrthographicSize = 4.5f;
        public static readonly Vector2 BoatPixelAnchor = new Vector2(1160, 750);
        public static readonly Vector2 ActorPixelAnchor = new Vector2(800, 820);
        public static readonly Vector2 ForegroundPixelAnchor = new Vector2(650, 820);
        public static readonly Rect LocalLane = Rect.MinMaxRect(-4, -2.7f, 2, -.6f);
        public static Vector3 FromPixel(Vector2 pixel) => new Vector3(pixel.x / PixelsPerUnit - 8, 4.5f - pixel.y / PixelsPerUnit, 0);
    }

    public sealed class RepairSceneView : MonoBehaviour
    {
        private SpriteRenderer actor, boat, foreground;
        private Sprite[] idle;
        private Sprite damaged, repaired;
        private Vector3 destination;
        private bool moving, visibleBoat, working;
        public Camera SceneCamera { get; private set; }
        public int ActorSortingOrder => actor.sortingOrder;
        public int ForegroundSortingOrder => foreground.sortingOrder;
        public Vector3 ActorFoot => actor.transform.position;
        public Vector3 BoatFoot => boat.transform.position;
        public Vector3 ForegroundFoot => foreground.transform.position;
        public bool WorkingFeedbackActive => working;

        public void Initialize()
        {
            var cameraObject = new GameObject("Fixed2DCamera"); SceneCamera = cameraObject.AddComponent<Camera>();
            SceneCamera.orthographic = true; SceneCamera.orthographicSize = RepairSceneSpec.OrthographicSize;
            SceneCamera.transform.position = new Vector3(0, 0, -10); SceneCamera.clearFlags = CameraClearFlags.SolidColor;
            SceneCamera.backgroundColor = new Color(.18f, .25f, .21f); SceneCamera.nearClipPlane = .1f; SceneCamera.farClipPlane = 30;
            var foundation = MakeSprite("Foundation", Load("base"), Vector3.zero, 0);
            foundation.transform.localScale = new Vector3(16 / foundation.sprite.bounds.size.x, 9 / foundation.sprite.bounds.size.y, 1);
            damaged = Load("boat-damaged"); repaired = Load("boat-repaired");
            boat = MakeSprite("ObservedBoat", damaged, RepairSceneSpec.FromPixel(RepairSceneSpec.BoatPixelAnchor), 10);
            SizeTo(boat, 530, 285, false);
            foreground = MakeSprite("ForegroundOccluder", Load("foreground"), RepairSceneSpec.FromPixel(RepairSceneSpec.ForegroundPixelAnchor), 20);
            SizeTo(foreground, 100, 300, true);
            idle = new[] { Load("actor-idle-0"), Load("actor-idle-1") };
            actor = MakeSprite("ObservedActor", idle[0], RepairSceneSpec.FromPixel(RepairSceneSpec.ActorPixelAnchor), 30);
            SizeTo(actor, 0, 210, true); destination = actor.transform.position; UpdateSorting();
        }
        private static Sprite Load(string name) => Resources.Load<Sprite>("Sprint15/Art/" + name)
            ?? throw new InvalidOperationException("Required2Dasset missing: " + name);
        private SpriteRenderer MakeSprite(string name, Sprite sprite, Vector3 foot, int order)
        {
            var obj = new GameObject(name); obj.transform.SetParent(transform, false); obj.transform.position = foot;
            var renderer = obj.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = order;
            return renderer;
        }
        private static void SizeTo(SpriteRenderer target, float width, float height, bool preserveAspect)
        {
            var bounds = target.sprite.bounds.size; var scaleY = height / RepairSceneSpec.PixelsPerUnit / bounds.y;
            target.transform.localScale = new Vector3(preserveAspect ? scaleY : width / RepairSceneSpec.PixelsPerUnit / bounds.x, scaleY, 1);
        }
        public void Render(RepairPlayerView view)
        {
            visibleBoat = view.BoatVisible; boat.enabled = visibleBoat;
            working = view.ActivityStatus == "running";
            // Unknown receives the same neutral illustration regardless of hidden truth.
            var image = view.BoatCondition.HasValue && view.BoatCondition.Value >= 700 ? repaired : damaged;
            if (boat.sprite != image) { boat.sprite = image; SizeTo(boat, 530, 285, false); }
        }
        private void Update()
        {
            if (actor == null) return;
            // Frame time is used only to animate/draw local presentation, never to advance game time.
            var image = idle[(int)(Time.unscaledTime * (working ? 3 : 1.4f)) % idle.Length];
            if (actor.sprite != image) { actor.sprite = image; SizeTo(actor, 0, 210, true); }
            // A small pivot-at-feet tilt is presentation feedback using the two idle images,
            // not a claim that a full authored working animation has been produced.
            actor.transform.localRotation = Quaternion.Euler(0, 0, working ? Mathf.Sin(Time.unscaledTime * 3) * 2.5f : 0);
            if (moving)
            {
                actor.transform.position = Vector3.MoveTowards(actor.transform.position, destination, Time.unscaledDeltaTime * 2);
                moving = Vector3.Distance(actor.transform.position, destination) > .01f; UpdateSorting();
            }
        }
        private void UpdateSorting() => actor.sortingOrder = actor.transform.position.y > foreground.transform.position.y ? 15 : 30;
        public bool IsBoatAt(Vector2 screenPoint)
        {
            if (!visibleBoat) return false;
            var world = SceneCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 10));
            return boat.bounds.Contains(world);
        }
        public void MoveLocalFromScreen(Vector2 screenPoint)
        {
            var world = SceneCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 10));
            if (!RepairSceneSpec.LocalLane.Contains(new Vector2(world.x, world.y))) return;
            destination = new Vector3(world.x, world.y, 0); moving = true;
        }
        public void PlaceLocalFoot(Vector3 foot)
        {
            if (!RepairSceneSpec.LocalLane.Contains(new Vector2(foot.x, foot.y))) throw new ArgumentOutOfRangeException(nameof(foot));
            actor.transform.position = new Vector3(foot.x, foot.y, 0); destination = actor.transform.position; moving = false; UpdateSorting();
        }
    }
}
