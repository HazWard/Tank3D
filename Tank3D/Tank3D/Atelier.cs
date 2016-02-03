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
        const string TITRE = "Tank3D";
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
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            const float ÉCHELLE_OBJET = 0.03f;
            Vector3 positionObjet = new Vector3(0, 10, -50);
            Vector3 rotationObjet = new Vector3(0, 0, 0); // MathHelper.PiOver2

            Vector3 positionCaméra = new Vector3(0, 100, 250);
            Vector3 cibleCaméra = new Vector3(0, 0, -10);
            // Menu------------------------------------------------------------------------------------------------------------------------
            const float MARGE_TITRE = 0.05f;
            int largeurÉcran = Window.ClientBounds.Width;
            int hauteurÉcran = Window.ClientBounds.Height;
            int dimensionMin = NB_TUILES * (hauteurÉcran / NB_TUILES);
            int margeZoneJeu = (hauteurÉcran % NB_TUILES) / 2;
            Rectangle zoneJeu = new Rectangle(margeZoneJeu, margeZoneJeu, dimensionMin, dimensionMin);
            Rectangle zoneTitre = new Rectangle(hauteurÉcran/2, 0, largeurÉcran - hauteurÉcran, hauteurÉcran / NB_ZONES);
            Rectangle zoneMessage = new Rectangle(hauteurÉcran, hauteurÉcran / NB_ZONES, largeurÉcran - hauteurÉcran, hauteurÉcran / NB_ZONES);
            Rectangle zoneTrésor = new Rectangle(hauteurÉcran, hauteurÉcran / NB_ZONES * 5, (largeurÉcran - hauteurÉcran) / 4, hauteurÉcran / NB_ZONES);
            Rectangle zoneVies = new Rectangle(hauteurÉcran + (largeurÉcran - hauteurÉcran) / 4, hauteurÉcran / NB_ZONES * 5, (largeurÉcran - hauteurÉcran) / 2, hauteurÉcran / NB_ZONES);
            Rectangle zoneDialogue = new Rectangle(hauteurÉcran/2, hauteurÉcran / 4, largeurÉcran - hauteurÉcran, hauteurÉcran / 2);

            Components.Add(new TexteCentré(this, TITRE, "Arial20", zoneTitre, Color.Gold, MARGE_TITRE));
            Components.Add(new Menu(this, "FondMenu", "Arial20", zoneDialogue));
            //-------------------------------------------------------------------------------------------------------------------------------
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);
            CaméraJeu = new CaméraSubjective(this, positionCaméra, cibleCaméra, Vector3.Up, INTERVALLE_MAJ_STANDARD);
            Components.Add(CaméraJeu);

            Components.Add(new Afficheur3D(this));
            //Components.Add(new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "PetiteCarte", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD));
            //Components.Add(new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "CarteUn", "DétailsDésert", 3, INTERVALLE_MAJ_STANDARD));
            //Components.Add(new Terrain(this, 1f, Vector3.Zero, Vector3.Zero, new Vector3(200, 25, 200), "CarteTest", "DétailsTerrain", 5, INTERVALLE_MAJ_STANDARD));
            Components.Add(new AfficheurFPS(this, "Arial20", Color.Red, INTERVALLE_CALCUL_FPS));
            //Components.Add(new ModèleMobile(this, "ship", ÉCHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_MAJ_STANDARD));

            //Services.AddService(typeof(Random), new Random());
            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<Model>), new RessourcesManager<Model>(this, "Modèles"));
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Caméra), CaméraJeu);
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

