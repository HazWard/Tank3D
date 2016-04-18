using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class CaméraSubjective : Caméra, IActivable
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float INCRÉMENT_DÉPLACEMENT = 0.5f;
        //const float ACCÉLÉRATION = 0.001f;
        const float VITESSE_INITIALE_ROTATION = 5f;
        const float VITESSE_INITIALE_TRANSLATION = 0.5f;
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
        const float RAYON_COLLISION = 1f;

        Vector3 Direction { get; set; }
        Vector3 Latéral { get; set; }
        Vector3 Alignation { get; set; }
        Vector3 Rotation { get; set; }
        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }

        float IncrémentAngleRotation { get; set; }
        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        Joueur CibleJoueur { get; set; }

        bool estEnZoom;
        bool EstEnZoom
        {
            get { return estEnZoom; }
            set
            {
                float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
                estEnZoom = value;
                if (estEnZoom)
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
                else
                {
                    CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
                }
            }
        }

        public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, float intervalleMAJ)
            : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            Cible = cible;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, cible, orientation);
            EstEnZoom = false;
        }

        public override void Initialize()
        {
            VitesseRotation = VITESSE_INITIALE_ROTATION;
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            IncrémentAngleRotation = (MathHelper.PiOver2 * IntervalleMAJ);
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
        }

        protected override void CréerPointDeVue()
        {
            Latéral = Vector3.Normalize(Vector3.Cross(OrientationVerticale, Direction));
            Direction = Vector3.Normalize(Cible - Position);
            OrientationVerticale = Vector3.Normalize(OrientationVerticale);
            Vue = Matrix.CreateLookAt(Position, Cible, OrientationVerticale);
            GénérerFrustum();
        }

        protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
        {
            Position = position;
            Cible = cible;
            Direction = Vector3.Normalize(Cible - Position);
            OrientationVerticale = orientation;
            OrientationVerticale = Vector3.Normalize(OrientationVerticale);
            Latéral = Vector3.Normalize(Vector3.Cross(OrientationVerticale, Direction));
            CréerPointDeVue();
        }

        public override void Update(GameTime gameTime)
        {
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            GestionClavier();

            if (CibleJoueur == null)
            {
                CibleJoueur = Game.Services.GetService(typeof (Joueur)) as Joueur;
            }

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (CibleJoueur.EstEnCollision)
                {

                }
                else
                {
                    //GérerAccélération();
                    GestionTouches();
                    GérerRotation();
                    CréerPointDeVue();
                    TempsÉcouléDepuisMAJ = 0;
                }
            }
            base.Update(gameTime);
        }

        void GestionTouches()
        {
            float déplacement = GérerTouche(Keys.W) - GérerTouche(Keys.S);
            float rotation = GérerTouche(Keys.D) - GérerTouche(Keys.A);
            if (déplacement != 0 || rotation != 0)
            {
                ModificationParamètres(déplacement, rotation);
            }
        }

        void ModificationParamètres(float déplacement, float rotation)
        {
            float rotationFinal = Rotation.Y - IncrémentAngleRotation * rotation;
            float posX = déplacement * (float)Math.Sin(rotationFinal);
            float posY = déplacement * (float)Math.Cos(rotationFinal);
            Vector2 déplacementFinal = new Vector2(posX, posY);
            Rotation = new Vector3(Rotation.X, rotationFinal, Rotation.Z);
            Position = new Vector3(Position.X - déplacementFinal.X, Position.Y, Position.Z - déplacementFinal.Y);
            Cible = new Vector3(Cible.X - déplacementFinal.X, Cible.Y, Cible.Z - déplacementFinal.Y);

        }

        private float GérerTouche(Keys touche)
        {
            return GestionInput.EstEnfoncée(touche) ? INCRÉMENT_DÉPLACEMENT : 0;
        }

        private void GérerDéplacement(float déplacement, float rotation)
        {
            Vector3 nouvellePosition = Position;
            Vector3 nouvelleCible = Cible;
            float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
            //float déplacementLatéral = (GérerTouche(Keys.A) - GérerTouche(Keys.D)) * VitesseTranslation;
            float rotationFinal = Rotation.Y - IncrémentAngleRotation * rotation;
            float posX = déplacement * (float)Math.Sin(rotationFinal);
            float posY = déplacement * (float)Math.Cos(rotationFinal);
            Vector2 déplacementFinal = new Vector2(posX, posY);

            if (déplacementDirection != 0f)
            {
                nouvellePosition += Vector3.Forward * déplacementDirection;
                nouvelleCible += Vector3.Forward * déplacementDirection;
            }
            //Cible = new Vector3(Position.X - déplacementFinal.X, Position.Y, Position.Z - déplacementFinal.Y);
            Position = nouvellePosition;
            //Cible = nouvelleCible;
        }

        private void GérerRotation()
        {
            float nb = GérerTouche(Keys.D) - GérerTouche(Keys.A);
            if (nb != 0)
            {
                Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(OrientationVerticale, (DELTA_LACET * nb) * VitesseRotation));
            }
        }

        private void GestionClavier()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Z))
            {
                EstEnZoom = !EstEnZoom;
            }
        }
        //private void GérerAccélération()
        //{
        //    int valAccélération = (GérerTouche(Keys.Subtract) + GérerTouche(Keys.OemMinus)) - (GérerTouche(Keys.Add) + GérerTouche(Keys.OemPlus));
        //    if (valAccélération != 0)
        //    {
        //        IntervalleMAJ += ACCÉLÉRATION * valAccélération;
        //        IntervalleMAJ = MathHelper.Max(INTERVALLE_MAJ_STANDARD, IntervalleMAJ);
        //    }
        //}

        public void ModifierActivation()
        {
            base.Enabled = !base.Enabled;
        }
    }
}
