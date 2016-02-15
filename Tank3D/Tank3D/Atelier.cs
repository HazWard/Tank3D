﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AtelierXNA
{
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        //--------------------------------------------------------------------------
        const string TITRE = "Tank 3D";
        const int NB_TUILES = 5;
        const int NB_ZONES = NB_TUILES + 1;
        //---------------------------------------------------------------------------

        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }

        Caméra CaméraJeu { get; set; }
        InputManager GestionInput { get; set; }

        public Atelier()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            PériphériqueGraphique.IsFullScreen = false;
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            const float ÉCHELLE_OBJET = 0.005f;
            const float ÉCHELLE_TERRAIN = 1f;
            Vector3 positionObjet = new Vector3(0, 10, 100);
            Vector3 positionAI = new Vector3(-20, 10, 50);
            Vector3 positionTerrain = new Vector3(0, 0, 0);
            Vector3 rotationObjet = new Vector3(0, 0, 0); // MathHelper.PiOver2
            Vector3 positionCaméraSubjective = new Vector3(0, 15, 15);
            Vector3 positionCaméra = new Vector3(0, 100, 250);
            Vector3 cibleCaméra = new Vector3(0, 0, -10);
            // Menu------------------------------------------------------------------------------------------------------------------------
            //const float MARGE_TITRE = 0.05f;
            int largeurÉcran = Window.ClientBounds.Width;
            int hauteurÉcran = Window.ClientBounds.Height;
            int dimensionMin = NB_TUILES * (hauteurÉcran / NB_TUILES);
            int margeZoneJeu = (hauteurÉcran % NB_TUILES) / 2;
            Rectangle zoneJeu = new Rectangle(margeZoneJeu, margeZoneJeu, dimensionMin, dimensionMin);
            Rectangle zoneTitre = new Rectangle(hauteurÉcran, 0, largeurÉcran - hauteurÉcran, hauteurÉcran / NB_ZONES);
            Rectangle zoneMessage = new Rectangle(hauteurÉcran, hauteurÉcran / NB_ZONES, largeurÉcran - hauteurÉcran, hauteurÉcran / NB_ZONES);
            Rectangle zoneDialogue = new Rectangle(hauteurÉcran, hauteurÉcran / 3, largeurÉcran - hauteurÉcran, hauteurÉcran / 2);

            //Components.Add(new TexteCentré(this, TITRE, "Arial20", zoneTitre, Color.Gold, MARGE_TITRE));
            //Components.Add(new Dialogue(this, "FondDialogue", "Arial20", zoneDialogue));

            Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, 6, -126), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-126, 6, 0), new Vector2(256, 50), new Vector2(10, 10), "desertDunesRéflexion", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -(MathHelper.PiOver2), 0), new Vector3(126, 6, 0), new Vector2(256, 50), new Vector2(10, 10), "desertDunesRéflexion", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.Pi, 0), new Vector3(0, 6, 126), new Vector2(256, 50), new Vector2(10, 10), "desertDunes", INTERVALLE_MAJ_STANDARD));
            Components.Add(new PlanTexturé(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, 31, 0), new Vector2(256, 256), new Vector2(10, 10), "ciel", INTERVALLE_MAJ_STANDARD));
            //-------------------------------------------------------------------------------------------------------------------------------
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);

            Components.Add(new Afficheur3D(this));

            Terrain TerrainJeu = new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "PetiteCarte", "DétailsDésertSable", 3, INTERVALLE_MAJ_STANDARD);
            Components.Add(TerrainJeu);
            Joueur joueur = new Joueur(this, "Tank", ÉCHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_MAJ_STANDARD);
            Components.Add(joueur);

            //Components.Add(new AI(this, "ship", ÉCHELLE_OBJET, rotationObjet, positionAI, INTERVALLE_MAJ_STANDARD, joueur));

            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<Model>), new RessourcesManager<Model>(this, "Modèles"));
            Services.AddService(typeof(InputManager), GestionInput);


            Services.AddService(typeof(Terrain), TerrainJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            GérerClavier();
            base.Update(gameTime);
        }

        private void GérerClavier()
        {
            if (GestionInput.EstEnfoncée(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GraphicsDevice.Clear(Color.Black);
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}

