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
    public class Dialogue : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int NB_ZONES_DIALOGUE = 3; //Cette constante doit valoir 3 au minimum
        string NomImageFond { get; set; }
        string NomPoliceDeCaractère { get; set; }
        Rectangle RectangleDialogue { get; set; }
        Texture2D ImageDeFond { get; set; }
        BoutonDeCommande BtnDémarrer { get; set; }
        BoutonDeCommande BtnPause { get; set; }
        BoutonDeCommande BtnInstructions { get; set; }
        BoutonDeCommande BtnQuitter { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        public Dialogue(Game jeu, string nomImageFond, String nomPoliceDeCaractère, Rectangle rectangleDialogue)
           : base(jeu)
        {
            NomImageFond = nomImageFond;
            NomPoliceDeCaractère = nomPoliceDeCaractère;
            RectangleDialogue = rectangleDialogue;
        }

        public override void Initialize()
        {
            int hauteurBouton = RectangleDialogue.Height / (NB_ZONES_DIALOGUE + 1);

            Vector2 PositionBouton = new Vector2(RectangleDialogue.X + RectangleDialogue.Width / 2f,
                                                 RectangleDialogue.Y + (NB_ZONES_DIALOGUE - 3) * hauteurBouton);
            BtnDémarrer = new BoutonDeCommande(Game, "Démarrer", NomPoliceDeCaractère, "BoutonNormal", "BoutonEnfoncé", PositionBouton, false, GérerPause);

            PositionBouton = new Vector2(RectangleDialogue.X + RectangleDialogue.Width / 2f,
                                                 RectangleDialogue.Y + (NB_ZONES_DIALOGUE - 2) * hauteurBouton);
            BtnPause = new BoutonDeCommande(Game, "Pause", NomPoliceDeCaractère, "BoutonNormal", "BoutonEnfoncé", PositionBouton, true, GérerPause);

            PositionBouton = new Vector2(RectangleDialogue.X + RectangleDialogue.Width / 2f,
                                     RectangleDialogue.Y + (NB_ZONES_DIALOGUE - 1) * hauteurBouton);
            BtnInstructions = new BoutonDeCommande(Game, "Instructions", NomPoliceDeCaractère, "BoutonNormal", "BoutonEnfoncé", PositionBouton, true, GérerPause);

            PositionBouton = new Vector2(RectangleDialogue.X + RectangleDialogue.Width / 2f,
                                                 RectangleDialogue.Y + NB_ZONES_DIALOGUE * hauteurBouton);
            BtnQuitter = new BoutonDeCommande(Game, "Quitter", NomPoliceDeCaractère, "BoutonNormal", "BoutonEnfoncé", PositionBouton, true, Quitter);

            Game.Components.Add(BtnDémarrer);
            Game.Components.Add(BtnPause);
            Game.Components.Add(BtnInstructions);
            Game.Components.Add(BtnQuitter);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            ImageDeFond = GestionnaireDeTextures.Find(NomImageFond);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.Draw(ImageDeFond, new Rectangle(RectangleDialogue.X,RectangleDialogue.Y - 10 ,RectangleDialogue.Width, RectangleDialogue.Height),Color.White);
            GestionSprites.End();
            base.Draw(gameTime);
        }

        public void GérerPause()
        {
            BtnDémarrer.EstActif = !BtnDémarrer.EstActif;
            BtnPause.EstActif = !BtnPause.EstActif;
            foreach (IActivable composant in Game.Components.Where(composant => composant is IActivable))
            {
                composant.ModifierActivation();
            }
        }

        public void Quitter()
        {
            Game.Exit();
        }

        public void DésactiverBoutons()
        {
            BtnDémarrer.EstActif = false;
            BtnPause.EstActif = false;
        }
    }
}
