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
    public class MenuPrincipal:Microsoft.Xna.Framework.Game
    {
        // Constantes
        const float POURCENTAGE_MARGE = 0.05f;
        const float INTERVALLE_CALCUL_FPS = 1f;
        
        Atelier Jeu { get; set; }
        CalculateurFPS Calculateur { get; set; }
        TexteCentré Titre { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        ArrièrePlan ImageArrièrePlan { get; set; }
        BoutonDeCommande BtnJouer { get; set; }
        BoutonDeCommande BtnInstructions { get; set; }
        BoutonDeCommande BtnOptions { get; set; }
        BoutonDeCommande BtnQuitter { get; set; }
        BoutonDeCommande BtnFermerFenêtre { get; set; }
        BoutonDeCommande BtnTextureTank { get; set; }
        BoutonDeCommande BtnCarteTerrain { get; set; }
        Instructions MenuInstructions { get; set; }
        public List<GameComponent> ListeGameComponents { get; set; }
        string NomTexture { get; set; }
        int NbEnnemis { get; set; }
        int Compteur { get; set; }

        public MenuPrincipal()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            PériphériqueGraphique.PreferredBackBufferWidth = 1920;
            PériphériqueGraphique.PreferredBackBufferHeight = 1080;
            PériphériqueGraphique.ApplyChanges();
            PériphériqueGraphique.IsFullScreen = false;
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GestionInput = new InputManager(this);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Calculateur = new CalculateurFPS(this, INTERVALLE_CALCUL_FPS);
            ListeGameComponents = new List<GameComponent>();
            NomTexture = "Veteran Tiger Body";
            NbEnnemis = 0;
            InitializeComponents();

            Components.Add(GestionInput);
            Components.Add(Calculateur);
            AddComponents();
            AddServices();

            base.Initialize();
        }

        void AddServices()
        {
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Services.AddService(typeof(CalculateurFPS), Calculateur);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<Model>), new RessourcesManager<Model>(this, "Modèles"));
        }

        void InitializeComponents()
        {
            Titre = new TexteCentré(this, "Tank 3D", "Arial20", new Rectangle(Window.ClientBounds.Width / 4, Window.ClientBounds.Height / 12, Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 8), Color.White, 0.1f);
            ImageArrièrePlan = new ArrièrePlan(this, "Background Tank");
            BtnJouer = new BoutonDeCommande(this, "Jouer", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 1.2f), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnInstructions = new BoutonDeCommande(this, "Instructions", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2 - 400, Window.ClientBounds.Height / 1.2f), true, new FonctionÉvénemtielle(AfficherInstructions));
            BtnOptions = new BoutonDeCommande(this, "Options", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2 + 400, Window.ClientBounds.Height / 1.2f), true, new FonctionÉvénemtielle(AfficherOptions));
            BtnQuitter = new BoutonDeCommande(this, "Quitter", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2 + 800, Window.ClientBounds.Height / 1.1f), true, new FonctionÉvénemtielle(QuitterJeu));
            BtnFermerFenêtre = new BoutonDeCommande(this, " X ", "Arial20", "BoutonRougeX", "BoutonBleuX", new Vector2(Window.ClientBounds.Width / 2 + 800, Window.ClientBounds.Height / 8), true, new FonctionÉvénemtielle(Retour));
        }

        void AddComponents()
        {
            Components.Add(Titre);
            Components.Add(ImageArrièrePlan);
            Components.Add(BtnJouer);
            Components.Add(BtnInstructions);
            Components.Add(BtnOptions);
            Components.Add(BtnQuitter);
            AddComponentsToList();
        }

        void AddComponentsToList()
        {
            ListeGameComponents.Add(Titre);
            ListeGameComponents.Add(ImageArrièrePlan);
            ListeGameComponents.Add(BtnJouer);
            ListeGameComponents.Add(BtnInstructions);
            ListeGameComponents.Add(BtnOptions);
            ListeGameComponents.Add(BtnQuitter);
        }

        void RemoveComponents()
        {
            Components.Remove(Titre);
            Components.Remove(ImageArrièrePlan);
            Components.Remove(BtnJouer);
            Components.Remove(BtnInstructions);
            Components.Remove(BtnOptions);
            Components.Remove(BtnQuitter);
            if (Components.Contains(Jeu))
            {
                Components.Remove(Jeu);
            }
        }

        public void ModifyComponents(bool créer, List<GameComponent> listeGameComponents)
        {
            if (créer)
            {
                foreach (DrawableGameComponent gc in listeGameComponents)
                {
                    gc.Visible = true;
                    gc.Enabled = true;
                }
            }
            else
            {
                foreach (DrawableGameComponent gc in listeGameComponents)
                {
                    gc.Visible = false;
                    gc.Enabled = false;
                }
            }
        }

        void DémarrerJeu() 
        {
            ModifyComponents(false, ListeGameComponents);
            Jeu = new Atelier(this, ListeGameComponents, NomTexture, NbEnnemis);
            Components.Add(Jeu);
        }
        void QuitterJeu()
        {
            Exit();
        }

        void EffacerMenu()
        {
            Components.Remove(ImageArrièrePlan);
            foreach (GameComponent gc in Components)
            {
                if (gc is BoutonDeCommande)
                {
                    ListeGameComponents.Add(gc);
                }
            }
            foreach (BoutonDeCommande btn in ListeGameComponents)
            {
                btn.Enabled = false;
                btn.Visible = false;
                //Components.Remove(btn);
            }
        }

        void AfficherInstructions()
        {
            int marge = (int)(Window.ClientBounds.Width * POURCENTAGE_MARGE);
            int pos = marge / 2;

            MenuInstructions = new Instructions(this, "FondInstructions", new Rectangle(pos, pos, Window.ClientBounds.Width - marge,
                                                Window.ClientBounds.Height - marge));
            Components.Add(MenuInstructions);
            Components.Add(BtnFermerFenêtre);
        }

        void AfficherOptions()
        {
            int marge = (int)(Window.ClientBounds.Width * POURCENTAGE_MARGE);
            int pos = marge / 2;

            MenuInstructions = new Instructions(this, "FondInstructions", new Rectangle(pos, pos, Window.ClientBounds.Width - marge,
                                                Window.ClientBounds.Height - marge));
            BtnTextureTank = new BoutonDeCommande(this, "Choisir texture du tank", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 4, Window.ClientBounds.Height / 4), true, new FonctionÉvénemtielle(IllustrerTextures));
            Components.Add(MenuInstructions);
            Components.Add(BtnFermerFenêtre);
            Components.Add(BtnTextureTank);
        }

        void IllustrerTextures()
        {

        }

        void Retour()
        {
            Components.Remove(MenuInstructions);
            Components.Remove(BtnFermerFenêtre);
            Components.Remove(BtnTextureTank);
        }

        void ArrêterJeu()
        {
            foreach (GameComponent gc in Components)
            {
                if (gc is IActivable)
                {
                    gc.Enabled = false;
                }
            }
        }
    }
}
