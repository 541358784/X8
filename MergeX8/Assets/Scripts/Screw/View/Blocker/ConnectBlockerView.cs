using System.Collections.Generic;
using DG.Tweening;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    [ViewAsset("Screw/Prefabs/Common/ConnectLine")]
    public class ConnectBlockerView : BaseBlockerView
    {
        public class Particle
        {
            public Vector3 position;
            public Vector3 oldPosition;
            public bool locked;
        }

        public class Stick
        {
            public Particle particleA;
            public Particle particleB;
            public float length;

            public Stick(Particle a, Particle b)
            {
                particleA = a;
                particleB = b;
                length = (a.position - b.position).magnitude;
            }
        }

        private float gravity = 30f;
        private int stiffness = 10;

        [UIBinder("")] private LineRenderer lineRenderer;

        private List<Particle> particles = new List<Particle>();
        private List<Stick> sticks = new List<Stick>();

        // [0]是终点，[1]是起点
        private List<ScrewView> _screwViews;

        private Tween destroyTween;

        private bool isConnect = true;
        public void SetConnectScrewViews(List<ScrewView> screwViews, ScrewModel model)
        {
            root.gameObject.layer = context.GetLayer(model.LayerId);
            _screwViews = screwViews;

            root.position = model.Position;
            lineRenderer.sortingOrder = model.LayerId * 1000 + lineRenderer.sortingOrder;

            Initialization(model.LayerId);
            
            UpdateScheduler.Instance.HookLateUpdate(LateUpdate);
            UpdateScheduler.Instance.HookFixedUpdate(FixedUpdate);
        }

        public void DisConnect()
        {
            if (!isConnect)
                return;
            isConnect = false;
            particles[0].locked = false;
            particles[particles.Count - 1].locked = false;

            destroyTween = DOVirtual.DelayedCall(1, Destroy);
        }

        public override void Destroy()
        {
            if (destroyTween != null)
                destroyTween.Kill();

            UpdateScheduler.Instance.UnhookLateUpdate(LateUpdate);
            UpdateScheduler.Instance.UnhookFixedUpdate(FixedUpdate);
            base.Destroy();
        }

        private void FixedUpdate()
        {
            Simulation();
        }

        private void LateUpdate()
        {
            Rendering();
        }

        private void Initialization(int layerId)
        {
            lineRenderer.positionCount = 20;

            var startPos = _screwViews[1].GetBodyPos();
            var endPos = _screwViews[0].GetBodyPos();

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 dot = startPos + ((endPos - startPos) / 20) * i;
                dot.z = layerId * -1;
                lineRenderer.SetPosition(i, dot);
                particles.Add(new Particle() {position = dot, oldPosition = dot});
            }

            for (int i = 0; i < particles.Count - 1; i++)
            {
                sticks.Add(new Stick(particles[i], particles[i + 1]));
            }

            particles[0].locked = true;
            particles[particles.Count - 1].locked = true;
        }

        private void Simulation()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                if (p.locked == false)
                {
                    Vector3 temp = p.position;
                    p.position = p.position + (p.position - p.oldPosition) +
                                 Time.fixedDeltaTime * Time.fixedDeltaTime * new Vector3(0, -gravity);
                    p.oldPosition = temp;
                }
                else
                {
                    if (i == 0)
                    {
                        p.oldPosition = p.position;
                        p.position = _screwViews[1].GetBodyPos();
                    }
                    else
                    {
                        p.oldPosition = p.position;
                        p.position = _screwViews[0].GetBodyPos();
                    }
                }
            }

            for (int i = 0; i < stiffness; i++)
            {
                for (int j = 0; j < sticks.Count; j++)
                {
                    Stick stick = sticks[j];

                    Vector3 delta = stick.particleB.position - stick.particleA.position;
                    float deltaLength = delta.magnitude;
                    float diff = (deltaLength - stick.length) / deltaLength;
                    if (stick.particleA.locked == false)
                        stick.particleA.position += 0.5f * diff * delta;
                    if (stick.particleB.locked == false)
                        stick.particleB.position -= 0.5f * diff * delta;
                }
            }
        }

        private void Rendering()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                lineRenderer.SetPosition(i, particles[i].position);
            }
        }
    }
}