using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
    public class ObjetDeBase : Microsoft.Xna.Framework.DrawableGameComponent, IModel
    {
        string NomModèle { get; set; }
        public BoundingSphere SphereCollision { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        protected Caméra CaméraJeu { get; set; }
        protected float Échelle { get; set; }
        protected Vector3 Rotation { get; set; }
        protected Vector3 Position { get; set; }

        protected Model Modèle { get; private set; }
        protected Matrix[] TransformationsModèle { get; private set; }
        protected Matrix Monde { get; set; }
        public bool EstEnCollision { get; set; }

        public ObjetDeBase(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu)
        {
            NomModèle = nomModèle;
            Position = positionInitiale;
            Échelle = échelleInitiale;
            Rotation = rotationInitiale;
        }

        public override void Initialize()
        {
            CalculerMonde();
            base.Initialize();
        }

        protected void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        protected override void LoadContent()
        {
            CaméraJeu = Game.Services.GetService(typeof(Caméra)) as Caméra;
            GestionnaireDeModèles = Game.Services.GetService(typeof(RessourcesManager<Model>)) as RessourcesManager<Model>;
            Modèle = GestionnaireDeModèles.Find(NomModèle);
            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameComponent gc in Game.Components)
            {
                if (gc is IModel && gc != this)
                {
                    ModèleMobile m = gc as ModèleMobile;
                    
                    if (SphereCollision.Intersects(m.SphereCollision))
                    {
                        if (this is Projectile)
                        {
                            ModèleMobile mm = m as ModèleMobile;
                            mm.AÉtéTiré = true;
                            Projectile p = this as Projectile;
                            p.EffacerProjectile(true, p.Position.X, p.Position.Z, p.Position.Y);
                        }
                        if (m is Projectile)
                        {
                            ModèleMobile mm = this as ModèleMobile;
                            mm.AÉtéTiré = true;
                            Projectile p = m as Projectile;
                            p.EffacerProjectile(true, p.Position.X, p.Position.Z, p.Position.Y);
                        }
                        EstEnCollision = true;
                        break;
                    }
                    else
                    {
                        EstEnCollision = false;
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh maille in Modèle.Meshes)
            {
                Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                {
                    BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                    effet.EnableDefaultLighting();
                    effet.Projection = CaméraJeu.Projection;
                    effet.View = CaméraJeu.Vue;
                    effet.World = mondeLocal;
                }
                maille.Draw();
            }
        }

        public virtual Matrix GetMonde()
        {
            return Monde;
        }
    }
}