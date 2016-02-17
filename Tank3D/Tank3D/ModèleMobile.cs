using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AtelierXNA
{
    public abstract class ModèleMobile : ObjetDeBase
    {
        // Constantes
        protected const float FACTEUR_ACCÉLÉRATION = 1f / 60f;
        protected const float INCRÉMENT_DÉPLACEMENT = 0.5f;
        protected const float HAUTEUR_DÉFAULT = 1f;

        // Propriétés
        protected float IncrémentAngleRotation { get; set; }
        protected float TempsÉcouléDepuisMAJ { get; set; }
        protected Terrain TerrainJeu { get; set; }
        protected float HauteurTerrain { get; set; }
        protected float IntervalleMAJ { get; set; }

        public ModèleMobile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            IncrémentAngleRotation = (MathHelper.TwoPi * IntervalleMAJ);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            TerrainJeu = Game.Services.GetService(typeof(Terrain)) as Terrain;
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GestionMouvements();
                //ÉcrireMeshes();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void ÉcrireMeshes()
        {
            foreach (ModelMesh m in Modèle.Meshes)
            {
                Console.WriteLine(m.Name);
            }
        }

        protected abstract void GestionMouvements();
    }
}