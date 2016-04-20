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
    public class Atelier : Microsoft.Xna.Framework.GameComponent
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        protected const float HAUTEUR_CAM_DÉFAULT = 10f;
        protected const float DISTANCE_POURSUITE = 20f;
        public const float ÉCHELLE_OBJET = 0.05f;

        MenuPause MenuPause { get; set; }
        List<GameComponent> ListeGameComponentsMenu { get; set; }
        List<GameComponent> ListeGameComponents { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        CaméraSubjective CaméraJoueur { get; set; }
        GestionnaireEnnemis GestionEnnemis { get; set; }
        NormalesManager GestionnaireDeNormales { get; set; }
        InputManager GestionInput { get; set; }
        Terrain TerrainJeu { get; set; }
        Joueur Utilisateur { get; set; }

        PlanTexturé PremierPlan { get; set; }
        PlanTexturé DeuxièmePlan { get; set; }
        PlanTexturé TroisièmePlan { get; set; }
        PlanTexturé QuatrièmePlan { get; set; }
        PlanTexturé Ciel { get; set; }

        Vector3 positionObjet { get; set; }
        Vector3 positionAI { get; set; }
        Vector3 positionTerrain { get; set; }
        Vector3 rotationObjet { get; set; }
        Vector3 positionCaméraSubjective { get; set; }
        Vector3 positionCaméra { get; set; }
        Vector3 cibleCaméra { get; set; }

        string NomModèleJoueur { get; set; }
        int NbEnnemis { get; set; }

        public Atelier(Game jeu, List<GameComponent> listeGameComponentsMenu, string nomModèleJoueur, int nbEnnemis)
            :base(jeu)
        {
            ListeGameComponentsMenu = listeGameComponentsMenu;
            NomModèleJoueur = nomModèleJoueur;
            NbEnnemis = nbEnnemis;
        }

        public override void Initialize()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

            positionObjet = new Vector3(0, 10, 0);
            positionAI = new Vector3(-20, 10, 50);
            positionTerrain = new Vector3(0, 0, 0);
            rotationObjet = new Vector3(0, 0, 0); // MathHelper.PiOver2
            positionCaméraSubjective = new Vector3(0, 15, 15);
            positionCaméra = new Vector3(0, 100, 250);
            cibleCaméra = new Vector3(0, 0, -10);

            ListeGameComponents = new List<GameComponent>();


            InitializeComponents();

            Game.Components.Add(GestionInput);
            Game.Components.Add(new Afficheur3D(Game));
            //Game.Components.Add(new Sprite(Game, "crosshairBon", new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2), 0.2f));

            if (Game.Services.GetService(typeof(Caméra)) as Caméra == null)
            {
                Game.Services.AddService(typeof(Caméra), CaméraJoueur);
            }

            AddComponents();
            Game.Components.Add(TerrainJeu);

            if (Game.Services.GetService(typeof(Terrain)) as Terrain == null)
            {
                Game.Services.AddService(typeof(Terrain), TerrainJeu);
            }

            Game.Components.Add(GestionEnnemis);
            Game.Components.Add(GestionnaireDeNormales);

            if (Game.Services.GetService(typeof(NormalesManager)) as NormalesManager == null)
            {
                Game.Services.AddService(typeof(NormalesManager), GestionnaireDeNormales);
            }

            Game.Components.Add(Utilisateur);

            base.Initialize();
        }

        void InitializeComponents()
        {
            GestionInput = new InputManager(Game);
            
            CaméraJoueur = new CaméraSubjective(Game, new Vector3(positionObjet.X, positionObjet.Y + HAUTEUR_CAM_DÉFAULT, positionObjet.Z + DISTANCE_POURSUITE),
                                               new Vector3(positionObjet.X, positionObjet.Y + 4, positionObjet.Z),
                                               Vector3.Up, INTERVALLE_MAJ_STANDARD);
            Game.Components.Add(CaméraJoueur);
            TerrainJeu = new Terrain(Game, 1f, Vector3.Zero, Vector3.Zero, new Vector3(256, 25, 256), "PetiteCarte", "DétailsDésert", 3, INTERVALLE_MAJ_STANDARD);
            GestionnaireDeNormales = new NormalesManager(Game);
            Utilisateur = new Joueur(Game, NomModèleJoueur, ÉCHELLE_OBJET, rotationObjet, positionObjet, INTERVALLE_MAJ_STANDARD);
            if (Game.Services.GetService(typeof(Joueur)) == null)
            {
                Game.Services.AddService(typeof(Joueur), Utilisateur);
            }
            GestionEnnemis = new GestionnaireEnnemis(Game, Utilisateur, TerrainJeu, NbEnnemis, ÉCHELLE_OBJET, INTERVALLE_MAJ_STANDARD);
            MenuPause = new MenuPause(Game, ListeGameComponentsMenu, ListeGameComponents, GestionEnnemis);
            //PremierPlan = new PlanTexturé(Game, 1f, Vector3.Zero, new Vector3(0, 6, -126), new Vector2(256, 60), new Vector2(10, 10), "desertDunes", INTERVALLE_MAJ_STANDARD);
            //DeuxièmePlan = new PlanTexturé(Game, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-126, 6, 0), new Vector2(256, 60), new Vector2(10, 10), "desertDunesRéflexion", INTERVALLE_MAJ_STANDARD);
            //TroisièmePlan = new PlanTexturé(Game, 1f, new Vector3(0, -(MathHelper.PiOver2), 0), new Vector3(126, 6, 0), new Vector2(256, 60), new Vector2(10, 10), "desertDunesRéflexion", INTERVALLE_MAJ_STANDARD);
            //QuatrièmePlan = new PlanTexturé(Game, 1f, new Vector3(0, MathHelper.Pi, 0), new Vector3(0, 6, 126), new Vector2(256, 60), new Vector2(10, 10), "desertDunes", INTERVALLE_MAJ_STANDARD);
            //Ciel = new PlanTexturé(Game, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, 35, 0), new Vector2(256, 256), new Vector2(10, 10), "ciel", INTERVALLE_MAJ_STANDARD);
        }

        void AddComponents()
        {
            AddTextures();
            AddComponentsToList();
        }

        void AddComponentsToList()
        {
            ListeGameComponents.Add(CaméraJoueur);
            ListeGameComponents.Add(TerrainJeu);
            ListeGameComponents.Add(GestionEnnemis);
            ListeGameComponents.Add(Utilisateur);
            ListeGameComponents.Add(MenuPause);
            ListeGameComponents.Add(PremierPlan);
            ListeGameComponents.Add(DeuxièmePlan);
            ListeGameComponents.Add(TroisièmePlan);
            ListeGameComponents.Add(QuatrièmePlan);
            ListeGameComponents.Add(Ciel);
            ListeGameComponents.Add(this);
        }

        void AddTextures()
        {
            //Game.Components.Add(PremierPlan);
            //Game.Components.Add(DeuxièmePlan);
            //Game.Components.Add(TroisièmePlan);
            //Game.Components.Add(QuatrièmePlan);
            //Game.Components.Add(Ciel);
        }

        public override void Update(GameTime gameTime)
        {
            GérerClavier();
            base.Update(gameTime);
        }

        private void GérerClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Escape))
            {
                if (!Game.Components.Contains(MenuPause))
                {
                    Game.Components.Add(MenuPause);
                }
                else
                {
                    MenuPause.Play();
                }
                //Exit();
            }
        }
    }
}
