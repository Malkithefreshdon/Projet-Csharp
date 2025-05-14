using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projet.Modules;
using System;

namespace TestUnitaireCSharp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Client_AjouterCommande_AjouteCommandeALHistorique()
        {
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            Assert.IsNotNull(client, "Le client ne devrait pas être null");

            Commande commande = new Commande
            {
                Id = 1,
                Client = client,
                DateCommande = DateTime.Now,
                Prix = 100
            };
            Assert.IsNotNull(commande, "La commande ne devrait pas être null");

            client.AjouterCommande(commande);

            Assert.IsNotNull(client.HistoriqueCommandes, "L'historique des commandes ne devrait pas être null");
            Assert.AreEqual(1, client.HistoriqueCommandes.Count, "L'historique devrait contenir exactement une commande");
            Assert.AreSame(commande, client.HistoriqueCommandes[0], "La commande dans l'historique devrait être identique à celle ajoutée");
            Assert.AreEqual(100, client.HistoriqueCommandes[0].Prix, "Le prix de la commande devrait être correct");
        }

        [TestMethod]
        public void SystemeFidelite_DeterminerStatut_RetourneStatutCorrect()
        {
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            SystemeFidelite systeme = new SystemeFidelite();

            var statut1 = systeme.DeterminerStatut(client);

            Assert.AreEqual(SystemeFidelite.StatutFidelite.Standard, statut1);
        }

        [TestMethod]
        public void CommandeManager_AjouterCommande_ChauffeurDejaOccupe_LeveException()
        {
            CommandeManager manager = new CommandeManager();
            Client client = new Client("123", "Doe", "John", DateTime.Now.AddYears(-30), "1 rue Test");
            Salarie chauffeur = new Salarie
            {
                NumeroSecuriteSociale = "456",
                Nom = "Smith",
                Prenom = "Bob",
                Poste = "Chauffeur"
            };
            DateTime dateLivraison = DateTime.Now.AddDays(1);

            Assert.IsFalse(Commande.EstChauffeurDisponible(manager, null, dateLivraison));
        }

        [TestMethod]
        public void StatistiqueService_ObtenirCommandesParVille_RetourneDistributionCorrecte()
        {
            StatistiqueService service = new StatistiqueService(
                new ClientManager(),
                new CommandeManager(),
                new SalarieManager()
            );

            Dictionary<string, int> distribution = service.ObtenirCommandesParVille();

            Assert.IsNotNull(distribution);
        }


        [TestMethod]
        public void SalarieManager_AjouterSalarie_AjouteSalarieEtMAJOrganigramme()
        {
            SalarieManager manager = new SalarieManager();
            Salarie nouveauSalarie = new Salarie
            {
                NumeroSecuriteSociale = "789",
                Nom = "Brown",
                Prenom = "Charlie",
                Poste = "Chef d'équipe"
            };

            bool resultat = manager.AjouterSalarie(nouveauSalarie);

            Assert.IsFalse(resultat);
            Assert.IsNotNull(manager.RechercherParId("789"));
        }


    }
}
