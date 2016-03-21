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
    class MenuPrincipal:Microsoft.Xna.Framework.Game
    {
        // Constantes
        const float POURCENTAGE_MARGE = 0.05f;
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInput { get; set; }
        ArrièrePlan ImageArrièrePlan { get; set; }
        BoutonDeCommande BtnJouer { get; set; }
        BoutonDeCommande BtnInstructions { get; set; }
        BoutonDeCommande BtnOptions { get; set; }
        BoutonDeCommande BtnQuitter { get; set; }
        BoutonDeCommande BtnRetourMenuPrincipal { get; set; }
        List<GameComponent> Boutons { get; set; }
        Instructions MenuInstructions { get; set; }

        public MenuPrincipal()
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
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<Model>), new RessourcesManager<Model>(this, "Modèles"));
            Services.AddService(typeof(InputManager), GestionInput);

            Boutons = new List<GameComponent>();
            ImageArrièrePlan = new ArrièrePlan(this, "Background Tank");
            BtnJouer = new BoutonDeCommande(this, "Jouer", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(100, 400), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnInstructions = new BoutonDeCommande(this, "Instructions", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(230, 400), true, new FonctionÉvénemtielle(AfficherInstructions));
            BtnOptions = new BoutonDeCommande(this, "Options", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(380, 400), true, new FonctionÉvénemtielle(DémarrerJeu));
            BtnQuitter = new BoutonDeCommande(this, "Quitter", "Arial20", "BoutonRouge", "BoutonBleu", new Vector2(490, 400), true, new FonctionÉvénemtielle(QuitterJeu));
            BtnRetourMenuPrincipal = new BoutonDeCommande(this, " X ", "Arial20", "BoutonRougeX", "BoutonBleuX", new Vector2(750, 50), true, new FonctionÉvénemtielle(Retour));
            Components.Add(ImageArrièrePlan);
            Components.Add(BtnJouer);
            Components.Add(BtnInstructions);
            Components.Add(BtnOptions);
            Components.Add(BtnQuitter);
            base.Initialize();
        }

        void DémarrerJeu() 
        {
            EffacerMenu();
            Atelier jeu = new Atelier(this);
            Components.Add(jeu);
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
                    Boutons.Add(gc);
                }
            }
            foreach(BoutonDeCommande btn in Boutons)
            {
                Components.Remove(btn);
            }
        }
        void AfficherInstructions()
        {
            int marge = (int)(Window.ClientBounds.Width * POURCENTAGE_MARGE);
            int pos = marge / 2;

            MenuInstructions = new Instructions(this, "FondInstructions", new Rectangle(pos, pos, Window.ClientBounds.Width - marge,
                                                Window.ClientBounds.Height - marge));
            Components.Add(MenuInstructions);
            Components.Add(BtnRetourMenuPrincipal);
        }

        void Retour()
        {
            Components.Remove(MenuInstructions);
            Components.Remove(BtnRetourMenuPrincipal);
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
