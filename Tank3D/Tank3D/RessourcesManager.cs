using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{

    class RessourceInvalideException : ApplicationException { }
    public class RessourcesManager<T>
    {
        Game Jeu { get; set; }
        string RépertoireDesTextures { get; set; }
        List<RessourceDeBase<T>> ListeRessources { get; set; }

        public RessourcesManager(Game jeu, string répertoireDesTextures)
        {
           Jeu = jeu;
           RépertoireDesTextures = répertoireDesTextures;
           ListeRessources = new List<RessourceDeBase<T>>();
        }

        public void Add(string nom, T ressourceRecue)
        {
           RessourceDeBase<T> ressourceÀAjouter = new RessourceDeBase<T>(nom, ressourceRecue);
           if (!ListeRessources.Contains(ressourceÀAjouter))
           {
              ListeRessources.Add(ressourceÀAjouter);
           }
           else
           {
               throw new RessourceInvalideException();
           }
        }

        void Add(RessourceDeBase<T> textureÀAjouter)
        {
           textureÀAjouter.Load();
           ListeRessources.Add(textureÀAjouter);
        }

        public T Find(string nomTexture)
        {
           const int TEXTURE_PAS_TROUVÉE = -1;
           RessourceDeBase<T> ressourceÀRechercher = new RessourceDeBase<T>(Jeu.Content, RépertoireDesTextures, nomTexture);
           int indexRessource = ListeRessources.IndexOf(ressourceÀRechercher);
           if (indexRessource == TEXTURE_PAS_TROUVÉE)
           {
              Add(ressourceÀRechercher);
              indexRessource = ListeRessources.Count - 1;
           }
           return ListeRessources[indexRessource].Texture;
        }
    }
}
