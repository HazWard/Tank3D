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
        CaméraFixe CaméraMenu { get; set; }
        TexteCentré Titre { get; set; }
        TexteCentré Instruction1 { get; set; }
        TexteCentré Instruction2 { get; set; }
        TexteCentré Instruction3 { get; set; }
        TexteCentré Instruction4 { get; set; }
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
        ZoneContextuelle Contenu { get; set; }
        ObjetTournant DémoTexture { get; set; }
        Vector2 DimensionTitre { get; set; }
        Vector2 DimensionPhrase { get; set; }
        public List<GameComponent> ListeGameComponents { get; set; }
        string NomTexture { get; set; }
        int Marge { get; set; }
        int PositionZoneContenu { get; set; }
        int NbEnnemis { get; set; }
        int Compteur { get; set; }

        public MenuPrincipal()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            PériphériqueGraphique.IsFullScreen = false;
            //PériphériqueGraphique.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //PériphériqueGraphique.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
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
            CaméraMenu = new CaméraFixe(this, new Vector3(0, 0, 300), Vector3.Zero, Vector3.Up);
            Services.AddService(typeof(Caméra), CaméraMenu);
            ListeGameComponents = new List<GameComponent>();
            NomTexture = "Veteran Tiger Body";
            NbEnnemis = 1;
            AddServices();
            InitializeComponents();

            Components.Add(GestionInput);
            Components.Add(Calculateur);
            AddComponents();
            
            Marge = (int)(Window.ClientBounds.Width * POURCENTAGE_MARGE);
            PositionZoneContenu = Marge / 2;

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

        public void InitializeComponents()
        {
            RessourcesManager<SpriteFont>  GestionFont = Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            SpriteFont Font = GestionFont.Find("Arial20");

            DimensionTitre = Font.MeasureString("Tank 3D");

            Titre = new TexteCentré(this, "Tank 3D", "Arial20", new Rectangle(Window.ClientBounds.Width / 2 - 2 * (int)DimensionTitre.X,
                                                                              Window.ClientBounds.Height / 5,
                                                                              4 * (int)DimensionTitre.X, 
                                                                              4 * (int)DimensionTitre.Y), 
                                                                              Color.Black, 0.1f);

            DimensionPhrase = Font.MeasureString("Le but est de tirer tous les tanks ennemis en évitant de se faire tirer.");

            Instruction1 = new TexteCentré(this, "  Le but est de tirer tous les tanks ennemis en évitant de se faire tirer.", "Arial20", 
                                                                new Rectangle(Window.ClientBounds.Width / 8,
                                                                              2 * Window.ClientBounds.Height / 8,
                                                                              2 * (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.White, 0.1f);

            DimensionPhrase = Font.MeasureString("Vous pouvez contrôler la caméra grâce à la souris.");

            Instruction2 = new TexteCentré(this, "   Vous pouvez contrôler la caméra grâce à la souris.", "Arial20",
                                                                new Rectangle(Window.ClientBounds.Width / 8,
                                                                              3 * Window.ClientBounds.Height / 8,
                                                                              2 * (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.White, 0.1f);

            DimensionPhrase = Font.MeasureString("Vous pouvez contrôler les mouvements du tank avec les touches (W,A,S,D).");

            Instruction3 = new TexteCentré(this, "Vous pouvez contrôler les mouvements du tank avec les touches (W,A,S,D).", "Arial20",
                                                                new Rectangle(Window.ClientBounds.Width / 8,
                                                                              4 * Window.ClientBounds.Height / 8,
                                                                              2 * (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.White, 0.1f);

            DimensionPhrase = Font.MeasureString("Vous pouvez tirer un projectile avec le clic gauche de la souris.");

            Instruction4 = new TexteCentré(this, "  Vous pouvez tirer un projectile avec le clic gauche de la souris.", "Arial20",
                                                                new Rectangle(Window.ClientBounds.Width / 8,
                                                                              5 * Window.ClientBounds.Height / 8,
                                                                              2 * (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.White, 0.1f);
            ImageArrièrePlan = new ArrièrePlan(this, "Background Tank");
            BtnJouer = new BoutonDeCommande(this, "Jouer", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2, 4 * Window.ClientBounds.Height / 5f), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnInstructions = new BoutonDeCommande(this, "Instructions", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2 - (Window.ClientBounds.Width / 4), 4 * Window.ClientBounds.Height / 5f), true, new FonctionÉvénemtielle(AfficherInstructions));
            BtnOptions = new BoutonDeCommande(this, "Options", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 2 + (Window.ClientBounds.Width / 4), 4 * Window.ClientBounds.Height / 5f), true, new FonctionÉvénemtielle(AfficherOptions));
            BtnQuitter = new BoutonDeCommande(this, "Quitter", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(5 * Window.ClientBounds.Width / 6, Window.ClientBounds.Height / 5f), true, new FonctionÉvénemtielle(QuitterJeu));
            BtnFermerFenêtre = new BoutonDeCommande(this, " X ", "Arial20", "BoutonRougeX", "BoutonBleuX", new Vector2(9 * Window.ClientBounds.Width / 10, Window.ClientBounds.Height / 5), true, new FonctionÉvénemtielle(Retour));
        }

        void AddComponents()
        {
            Components.Add(ImageArrièrePlan);
            Components.Add(BtnJouer);
            Components.Add(BtnInstructions);
            Components.Add(BtnOptions);
            Components.Add(BtnQuitter);
            Components.Add(Titre);
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

        public static void ModifyComponents(bool créer, List<GameComponent> listeGameComponents)
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
            Services.RemoveService(typeof(Caméra));
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
            Contenu = new ZoneContextuelle(this, "FondInstructions", "Instructions", new Rectangle(PositionZoneContenu, PositionZoneContenu, Window.ClientBounds.Width - Marge,
                                                Window.ClientBounds.Height - Marge));
            Components.Add(Contenu);
            Components.Add(BtnFermerFenêtre);
            Components.Add(Instruction1);
            Components.Add(Instruction2);
            Components.Add(Instruction3);
            Components.Add(Instruction4);
        }

        void AfficherOptions()
        {
            Contenu = new ZoneContextuelle(this, "FondInstructions", "Options", new Rectangle(PositionZoneContenu, PositionZoneContenu, Window.ClientBounds.Width - Marge,
                                                Window.ClientBounds.Height - Marge));
            BtnTextureTank = new BoutonDeCommande(this, "Choix de la texture", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(Window.ClientBounds.Width / 4, Window.ClientBounds.Height / 4), true, new FonctionÉvénemtielle(IllustrerTextures));
            
            Components.Add(Contenu);
            Components.Add(BtnFermerFenêtre);
            Components.Add(BtnTextureTank);
        }

        void IllustrerTextures()
        {
            Components.Remove(DémoTexture);
            DémoTexture = new ObjetTournant(this, NomTexture, 0.5f, new Vector3(0, - MathHelper.PiOver2, 0), new Vector3(0, -50, 0));
            Components.Add(DémoTexture);
        }

        void Retour()
        {
            Components.Remove(Contenu);
            Components.Remove(BtnFermerFenêtre);
            Components.Remove(Instruction1);
            Components.Remove(Instruction2);
            Components.Remove(Instruction3);
            Components.Remove(Instruction4);
            Components.Remove(BtnTextureTank);
            Components.Remove(DémoTexture);
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
