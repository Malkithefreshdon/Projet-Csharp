using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projet.Modules;

namespace TestUnitaireCSharp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Client_AjouterCommande_AjouteCommandeALHistorique()
        {
            // Arrange
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            Commande commande = new Commande
            {
                Id = 1,
                Client = client,
                DateCommande = DateTime.Now,
                Prix = 100
            };

            // Act
            client.AjouterCommande(commande);

            // Assert
            Assert.IsNotNull(client.HistoriqueCommandes, "L'historique des commandes ne devrait pas être null");
            Assert.AreEqual(1, client.HistoriqueCommandes.Count, "L'historique devrait contenir exactement une commande");
            Assert.AreSame(commande, client.HistoriqueCommandes[0], "La commande dans l'historique devrait être identique à celle ajoutée");
        }
    }
}
