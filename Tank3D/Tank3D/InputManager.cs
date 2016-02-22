using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
   public class InputManager : Microsoft.Xna.Framework.GameComponent
   {
      Keys[] AnciennesTouches { get; set; }
      Keys[] NouvellesTouches { get; set; }
      KeyboardState ÉtatClavier { get; set; }
      public MouseState NouvelÉtatSouris { get; set; }
      public MouseState AncienÉtatSouris { get; set; }

      public InputManager(Game game)
         : base(game)
      { }

      public override void Initialize()
      {
         AnciennesTouches = new Keys[0];
         NouvellesTouches = new Keys[0];
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         AnciennesTouches = NouvellesTouches;
         ÉtatClavier = Keyboard.GetState();
         NouvellesTouches = ÉtatClavier.GetPressedKeys();

         AncienÉtatSouris = NouvelÉtatSouris;
         NouvelÉtatSouris = Mouse.GetState();
         base.Update(gameTime);
      }

      public bool EstClavierActivé
      {
         get { return NouvellesTouches.Length > 0; }
      }
      public bool EstSourisActive 
      { 
         get { return NouvelÉtatSouris != AncienÉtatSouris;} 
      }
      public bool EstAncienClicDroit()
      {
          return NouvelÉtatSouris.RightButton == ButtonState.Pressed && !EstSourisActive;
      }
      public bool EstAncienClicGauche()
      {
          return NouvelÉtatSouris.LeftButton == ButtonState.Pressed && !EstSourisActive;
      }
      public bool EstNouveauClicDroit()
      {
          return NouvelÉtatSouris.RightButton == ButtonState.Pressed &&
                 AncienÉtatSouris.RightButton == ButtonState.Released;
      }
      public bool EstNouveauClicGauche()
      {
          return NouvelÉtatSouris.LeftButton == ButtonState.Pressed &&
                 AncienÉtatSouris.LeftButton == ButtonState.Released;
      }
      public Point GetPositionSouris()
      {
         return new Point(NouvelÉtatSouris.X, NouvelÉtatSouris.Y);
      }

      public bool EstEnfoncée(Keys touche)
      {
         return ÉtatClavier.IsKeyDown(touche);
      }

      public bool EstNouvelleTouche(Keys touche)
      {
         int NbTouches = AnciennesTouches.Length;
         bool EstNouvelleTouche = ÉtatClavier.IsKeyDown(touche);
         int i = 0;
         while (i < NbTouches && EstNouvelleTouche)
         {
            EstNouvelleTouche = AnciennesTouches[i] != touche;
            ++i;
         }
         return EstNouvelleTouche;
      }
      public MouseState GetAncienÉtatSouris()
      {
          return AncienÉtatSouris;
      }
   }
}