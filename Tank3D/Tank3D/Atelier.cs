using System;
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
        public const float ÉCHELLE_OBJET = 0.05f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        GestionnaireEnnemis GestionEnnemis { get; set; }
        CalculateurFPS Calculateur { get; set; }
        Caméra CaméraJeu { get; set; }
        InputManager GestionInput { get; set; }
        Terrain TerrainJeu { get; set; }
        Joueur Utilisateur { get; set; }
        AI TankEnnemi { get; set; }
        Vector3 positionObjet { get; set; }
        Vector3 positionAI { get; set; }
        Vector3 positionTerrain { get; set; }
        Vector3 rotationObjet { get; set; }
        Vector3 positionCaméraSubjective { get; set; }
        Vector3 positionCaméra { get; set; }
        Vector3 cibleCaméra { get; set; }

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
            positionObjet = new Vector3(0, 10, 100);
            positionAI = new Vector3(-20, 10, 50);
            positionTerrain = new Vector3(0, 0, 0);
            rotationObjet = new Vector3(0, 0, 0); // MathHelper.PiOver2
            positionCaméraSubjective = new Vector3(0, 15, 15);
            positionCaméra = new Vector3(0, 100, 250);
            cibleCaméra = new Vector3(0, 0, -10);
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
            Calculateur = new CalculateurFPS(this, INTERVALLE_CALCUL_FPS);
            Components.Add(Calculateur);
            Services.AddService(typeof(CalculateurFPS), Calculateur);
            Components.Add(new Afficheur3D(this));

            TerrainJeu = new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "Homemade", "DétailsDésertSable", 3, INTERVALLE_MAJ_STANDARD);
            Components.Add(TerrainJeu);
            Components.Add(new Sprite(this, "crosshairBon", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2), 0.2f));
            Utilisateur = new Joueur(this, "Veteran Tiger Body", ÉCHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_MAJ_STANDARD);
            Components.Add(Utilisateur);
            //TankEnnemi = new AI(this, "Veteran Tiger NoColor", ÉCHELLE_OBJET, rotationObjet, positionAI, INTERVALLE_MAJ_STANDARD, Utilisateur);
            //Components.Add(TankEnnemi);
            GestionEnnemis = new GestionnaireEnnemis(this, Utilisateur, TerrainJeu, 5, ÉCHELLE_OBJET, INTERVALLE_MAJ_STANDARD);
            Components.Add(GestionEnnemis);
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

