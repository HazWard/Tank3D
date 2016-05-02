using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    public class ZoneContextuelle : ArrièrePlan
    {
        // Propriétés
        Rectangle Zone { get; set; }
        string TypeMenu { get; set; }
        CaméraFixe CaméraMenu { get; set; }
        TexteCentré Instruction1 { get; set; }
        TexteCentré Instruction2 { get; set; }
        TexteCentré Instruction3 { get; set; }
        TexteCentré Instruction4 { get; set; }

        ObjetTournant DémoTexture { get; set; }
        BoutonDeCommande BtnTextureBody { get; set; }
        BoutonDeCommande BtnTextureDesert { get; set; }
        BoutonDeCommande BtnTextureForest { get; set; }
        BoutonDeCommande BtnTextureSnow { get; set; }
        BoutonDeCommande BtnTextureTank { get; set; }
        Vector2 DimensionPhrase { get; set; }
        bool BoutonsTextureAjoutés { get; set; }
        string NomTexture { get; set; }

        public ZoneContextuelle(Game game, string nomImage, string typeMenu, Rectangle zone)
            : base(game, nomImage)
        {
            Zone = zone;
        }
        public override void Initialize()
        {
            // Boutons de changement de texture
            NomTexture = "Veteran Tiger Body";
            BoutonsTextureAjoutés = false;
            BtnTextureBody = new BoutonDeCommande(Game, "  O  ", "Arial20", "Tank_Body_nocolor", "BtnTextureSelect", new Vector2(Game.Window.ClientBounds.Width / 8, 2.5f * Game.Window.ClientBounds.Height / 6f), true, new FonctionÉvénemtielle(ChangeToBody));
            BtnTextureForest = new BoutonDeCommande(Game, "  O  ", "Arial20", "Tank_Body_Forest", "BtnTextureSelect", new Vector2(Game.Window.ClientBounds.Width / 8, 3f * Game.Window.ClientBounds.Height / 6f), true, new FonctionÉvénemtielle(ChangeToForest));
            BtnTextureDesert = new BoutonDeCommande(Game, "  O  ", "Arial20", "Tank_Body_Desert", "BtnTextureSelect", new Vector2(Game.Window.ClientBounds.Width / 8, 3.5f * Game.Window.ClientBounds.Height / 6f), true, new FonctionÉvénemtielle(ChangeToDesert));
            BtnTextureSnow = new BoutonDeCommande(Game, "  O  ", "Arial20", "Tank_Body_Snow", "BtnTextureSelect", new Vector2(Game.Window.ClientBounds.Width / 8, 4f * Game.Window.ClientBounds.Height / 6f), true, new FonctionÉvénemtielle(ChangeToSnow));
            CaméraMenu = new CaméraFixe(Game, new Vector3(0, 0, 300), Vector3.Zero, Vector3.Up);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.Draw(ImageDeFond, Zone, Color.White);
            AfficherContenu();
            GestionSprites.End();
        }

        void AfficherContenu()
        {
            switch (TypeMenu)
            {
                case "Instructions":
                    ÉcrireInstructions();
                    break;

                case "Options":
                    ÉcrireOptions();
                    break;
            }
        }

        void ÉcrireOptions()
        {
            BtnTextureTank = new BoutonDeCommande(Game, "Choix de la texture", "Arial20", "BoutonNormal", "BoutonEnfoncé",
                                    new Vector2(Game.Window.ClientBounds.Width / 4, Game.Window.ClientBounds.Height / 4),
                                    true, new FonctionÉvénemtielle(IllustrerTank));
            Game.Components.Add(BtnTextureTank);
            Game.Components.Add(DémoTexture);
            Game.Components.Add(BtnTextureBody);
            Game.Components.Add(BtnTextureDesert);
            Game.Components.Add(BtnTextureForest);
            Game.Components.Add(BtnTextureSnow);
        }

        void ÉcrireInstructions()
        {
            // --- Textes à afficher ---
            string texteInstruction1 = "Le but est de tirer tous les tanks ennemis en évitant de se faire tirer.";
            string texteInstruction2 = "Contrôlez la caméra avec la souris.";
            string texteInstruction3 = "Contrôler les mouvements du tank avec [W,A,S,D].";
            string texteInstruction4 = "Tirer des projectiles avec le clic gauche.";
            // -------

            RessourcesManager<SpriteFont> GestionFont = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            SpriteFont Font = GestionFont.Find("Arial20");

            DimensionPhrase = Font.MeasureString(texteInstruction1);

            Instruction1 = new TexteCentré(Game, texteInstruction1, "Arial20",
                                                                new Rectangle(Game.Window.ClientBounds.Width / 5,
                                                                              2 * Game.Window.ClientBounds.Height / 8,
                                                                              (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.Black, 0.1f);

            DimensionPhrase = Font.MeasureString(texteInstruction2);

            Instruction2 = new TexteCentré(Game, texteInstruction2, "Arial20",
                                                                new Rectangle(Game.Window.ClientBounds.Width / 5,
                                                                              3 * Game.Window.ClientBounds.Height / 8,
                                                                              (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.Black, 0.1f);

            DimensionPhrase = Font.MeasureString(texteInstruction3);

            Instruction3 = new TexteCentré(Game, texteInstruction3, "Arial20",
                                                                new Rectangle(Game.Window.ClientBounds.Width / 5,
                                                                              4 * Game.Window.ClientBounds.Height / 8,
                                                                              (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.Black, 0.1f);

            DimensionPhrase = Font.MeasureString(texteInstruction4);

            Instruction4 = new TexteCentré(Game, texteInstruction4, "Arial20",
                                                                new Rectangle(Game.Window.ClientBounds.Width / 5,
                                                                              5 * Game.Window.ClientBounds.Height / 8,
                                                                              (int)DimensionPhrase.X,
                                                                              2 * (int)DimensionPhrase.Y),
                                                                              Color.Black, 0.1f);
            Game.Components.Add(Instruction1);
            Game.Components.Add(Instruction2);
            Game.Components.Add(Instruction3);
            Game.Components.Add(Instruction4);
        }

        public void EffacerContenu()
        {
            switch (TypeMenu)
            {
                case "Instructions":
                    Game.Components.Remove(Instruction1);
                    Game.Components.Remove(Instruction2);
                    Game.Components.Remove(Instruction3);
                    Game.Components.Remove(Instruction4);
                    break;

                case "Options":
                    Game.Components.Remove(BtnTextureTank);
                    Game.Components.Remove(DémoTexture);
                    Game.Components.Remove(BtnTextureBody);
                    Game.Components.Remove(BtnTextureDesert);
                    Game.Components.Remove(BtnTextureForest);
                    Game.Components.Remove(BtnTextureSnow);
                    break;
            }
        }

        void IllustrerTank()
        {
            if (!BoutonsTextureAjoutés)
            {
                Game.Components.Add(BtnTextureBody);
                Game.Components.Add(BtnTextureDesert);
                Game.Components.Add(BtnTextureForest);
                Game.Components.Add(BtnTextureSnow);
                BoutonsTextureAjoutés = true;
            }

            if (Game.Services.GetService(typeof(Caméra)) == null)
            {
                Game.Services.AddService(typeof(Caméra), CaméraMenu);
            }

            Game.Components.Remove(DémoTexture);
            DémoTexture = new ObjetTournant(Game, NomTexture, 0.5f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(0, -50, 0));
            Game.Components.Add(DémoTexture);
        }

        void ChangeToBody()
        {
            NomTexture = "Veteran Tiger Body";
            IllustrerTank();
        }
        void ChangeToDesert()
        {
            NomTexture = "Veteran Tiger Desert";
            IllustrerTank();
        }
        void ChangeToSnow()
        {
            NomTexture = "Veteran Tiger Snow";
            IllustrerTank();
        }
        void ChangeToForest()
        {
            NomTexture = "Veteran Tiger Forest";
            IllustrerTank();
        }
    }
}
