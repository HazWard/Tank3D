using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
   public class InputManager : Microsoft.Xna.Framework.GameComponent
   {
      Keys[] AnciennesTouches { get; set; }
      Keys[] NouvellesTouches { get; set; }
      KeyboardState ÉtatClavier { get; set; }
      MouseState ÉtatSouris { get; set; }
      MouseState NouvelÉtatSouris { get; set; }
      MouseState AncienÉtatSouris { get; set; }

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
         ÉtatSouris = Mouse.GetState();
         NouvelÉtatSouris = ÉtatSouris;
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
          return ÉtatSouris.RightButton == ButtonState.Pressed && !EstSourisActive;
      }
      public bool EstAncienClicGauche()
      {
          return ÉtatSouris.LeftButton == ButtonState.Pressed && !EstSourisActive;
      }
      public bool EstNouveauClicDroit()
      {
          return ÉtatSouris.RightButton == ButtonState.Pressed && EstSourisActive;
      }
      public bool EstNouveauClicGauche()
      {
          return ÉtatSouris.LeftButton == ButtonState.Pressed && EstSourisActive;
      }
      public Point GetPositionSouris()
      {
         return new Point(ÉtatSouris.X, ÉtatSouris.Y);
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