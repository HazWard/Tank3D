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
        protected const float HAUTEUR_DÉFAULT = 0.5f;

        // Propriétés
        protected InputManager GestionInput { get; set; }
        protected float IncrémentAngleRotation { get; set; }
        protected float TempsÉcouléDepuisMAJ { get; set; }
        protected Terrain TerrainJeu { get; set; }
        protected NormalesManager GestionnaireDeNormales { get; set; }
        protected float NouvelleHauteurTerrain { get; set; }
        protected float AncienneHauteurTerrain { get; set; }
        protected float IntervalleMAJ { get; set; }
        protected Point nouvellesCoords { get; set; }
        protected bool AÉtéCliqué { get; set; }

        public ModèleMobile(Game jeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ)
            : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
        {
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            TempsÉcouléDepuisMAJ = 0;
            IncrémentAngleRotation = MathHelper.TwoPi * IntervalleMAJ;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            TerrainJeu = Game.Services.GetService(typeof(Terrain)) as Terrain;
            GestionnaireDeNormales = Game.Services.GetService(typeof(NormalesManager)) as NormalesManager;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected bool EstHorsDesBornes(Point coords)
        {
            bool estHorsDesBornes = false;

            if (coords.X >= TerrainJeu.Extrêmes - 5 || coords.Y >= TerrainJeu.Extrêmes - 5 || 
                coords.X <= 5 || coords.Y <= 5)
            {
                estHorsDesBornes = true;
            }

            return estHorsDesBornes;
        }
    }
}