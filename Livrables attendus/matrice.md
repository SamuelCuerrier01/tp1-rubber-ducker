| Élément           | Référence             | Lien                                                                                                                                           |
| :---------------- | :-------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-01                 | ajouter une nouvelle tâche                                                                                                                     |
| Cas d'utilisation | UC2                   | Ajout de tâche par l'utilisateur                                                                                                               |
| Conception        | SD-tache-DB           | sequenceBD.uml                                                                                                                                 |
| Validation        | AC-01                 | le UI se met à jour après l'ajout d'une nouvelle tâche.                                                                                        |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-02                 | créer une nouvelle routine avec des modules.                                                                                                   |
| Cas d'utilisation | UC12                  | Ajout d'un module par l'utilisatieur                                                                                                           |
| Conception        | SD-module-DB          | sequenceBD.uml                                                                                                                                 |
| Validation        | AC-02                 | le UI se met à jour après l'ajout d'un nouveau module.                                                                                         |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-03                 | s'inscrire une première fois sur l'application                                                                                                 |
| Cas d'utilisation | UC9                   | S'incrire                                                                                                                                      |
| Conception        | SD-inscription        | planTache.uml                                                                                                                                  |
| Validation        | AC-03                 | l'application permet d'interagir avec l'app en enregistrant les informations pour l'utilisateur créé.                                          |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-04                 | se connecter sur l'application                                                                                                                 |
| Cas d'utilisation | UC10                  | Se connecter                                                                                                                                   |
| Conception        | SD-connexion          | planTache.uml                                                                                                                                  |
| Validation        | AC-04                 | l'application permet d'interagir avec l'app et de conserver les information pour quand on se reconnecte au compte.                             |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-05                 | Organiser les tâches dans des modules                                                                                                          |
| Cas d'utilisation | UC1                   | mettre les tâches dans des modules                                                                                                             |
| Conception        | SD-module-tache       | planTache.uml                                                                                                                                  |
| Validation        | AC-05                 | l'application permet d'avoir ses tâches dans des modules pour mieux les regroupers.                                                            |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-06                 | Supprimer une tâche                                                                                                                            |
| Cas d'utilisation | UC3                   | Suppression d'une tâche du module choisi                                                                                                       |
| Conception        | SD-suppression-tache  | planTache.uml                                                                                                                                  |
| Validation        | AC-06                 | l'application permet de supprimer une tâche de la base de donnée ce qui la fait disparaitre de l'interface utilisateur.                        |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-07                 | Terminer une tâche                                                                                                                             |
| Cas d'utilisation | UC4                   | Changement du statut de tâche à terminé                                                                                                        |
| Conception        | SD-terminer-tache     | planTache.uml                                                                                                                                  |
| Validation        | AC-07                 | le UI affiche l'état de la tâche qui est changé pour teminé.                                                                                   |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-08                 | Obtenir la progression du module                                                                                                               |
| Cas d'utilisation | UC5                   | Affichage de la progression des tâches termiées et non-terminées                                                                               |
| Conception        | SD-progression-module | planTache.uml                                                                                                                                  |
| Validation        | AC-08                 | le UI affiche la progression du module où les tâches font avancer la progression si elles sont terminées.                                      |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-09                 | Obtenir des suggestions de tâches provenant de l'intelligence artificielle                                                                     |
| Cas d'utilisation | UC6                   | Affichage des suggestions de tâches offertes par l'IA                                                                                          |
| Conception        | SD-suggestion         | planTache.uml                                                                                                                                  |
| Validation        | AC-09                 | le UI affiche les suggestions de tâches offertes par l'IA que le client peut choisir d'ajouter au module.                                      |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| story             | US-10                 | augmenter son streak pour les tâches faites de façon journalière                                                                               |
| cas d'utilisation | UC7                   | incrémentation du streak pour les tâches accomplises de façon journalière                                                                      |
| conception        | SD-streak             | plantache.uml                                                                                                                                  |
| validation        | AC-10                 | le ui met à jour son streak si une nouvelle tâches est faite à chaque jour.                                                                    |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-11                 | Obtenir des rappels de tâches                                                                                                                  |
| Cas d'utilisation | UC8                   | Envoi de rappel de tâche pour éviter un oubli                                                                                                  |
| Conception        | SD-rappel             | planTache.uml                                                                                                                                  |
| Validation        | AC-11                 | l'application envoi un rappel de tâche pour éviter d'oublier de faire la tâche avant sa date limite.                                           |
| :---------------- | :------------         | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| Story             | US-12                 | Changer la langue de l'interface utilisateur                                                                                                   |
| Cas d'utilisation | UC11                  | Changement de langue de l'interface                                                                                                            |
| Conception        | SD-langue             | planTache.uml                                                                                                                                  |
| Validation        | AC-12                 | l'application change la langue d'affichage des menus pour s'adapter à la demande UI.                                                           |