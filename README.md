# *Smash Reborn Turbo*

La suite tant attendue de *Smash Reborn* est là pour sa version ultime !

![](Assets/pictureimport/SmashRebornTitre.png)

## Description

*Smash Reborn Turbo* est toujours un jeu de plateforme dans lequel deux joueurs s'affrontent. Le joueur gagnant est celui réussissant à expulser l'autre joueur de la plateforme le plus grand nombre de fois. Chaque coup fait augmenter les pourcents de la cible, ce qui augmente la distance à laquelle elle est éjectée à chaque attaque.

## Commandes

**Joueur 1 : Randy**

- Gauche : `[Q]`
- Droite : `[D]`
- Bas : `[S]`
- Haut : `[Z]` (bon maintenant le bas est utile mais toujous pas le haut, flm de faire ce perso)
- Sauter : `[V]`
- Sprint : Nope
- Attaquer : `[C]`
- Attaque Speciale : `[B]` (il faut vraiment que je finisse pas faire Randy parce que là ça commence à devenir grave)
- Bouclier : `[F]`

**Joueur 2 : Bobby S. Lime**

- Gauche : `[K]`
- Droite : `[M]`
- Bas : `[L]`
- Haut : `[O]`
- Sauter : `[Right Shift]`
- Sprint : `[NopeNope]`
- Attaquer : `[!]`
- Attaque Speciale : `[Enter]`
- Bouclier : `[ù]`

Bas permet de descendre des plate formes ou de se baisser.
Les attaques implémentées sont:
Au sol: Forward Tilt (attaque et direction), Up Tilt (attaque et haut), Down Tilt (attaque et bas)
Dans les air: Neutral Air (attaque sans direction)
Spécial: Up Special (attaque spéciale et haut), Side Special (attaque spéciale et côté), Down Special (attaque spéciale et bas) et Neutral Special (attaque speciale sans direction)
Les attaques spéciales sont utilisables sur le sol et dans les airs.

## Instructions pour ajouter un personnage

**Préambule**

Il est conseillé de faire le kit du personnage avant de s'y mettre. Même si Riki peut aider, c'est à toi de programmer les attaque du personnage (en t'aidant de ce que j'ai déja fait).
Attention ! Il est conseillé utiliser les mêmes bases pour les noms ainsi que pour les variables qui concernent les attaques. Les movements et le sauts sont gérés dans des scripts conçus pour fonctionner sur tous les personnage. Le seul script à créer est le script d'attaque. Pour Bobby, il s'appèle est BobbyAtk, celui pour Randy est RandyAtk. Il faut le mettre sous la forme NomAtk pour les attaques pour que tout soit joli et standardisé.
Il est absolument capitale d'utiliser **le même script charavalues, Charamov, charaper, charavalues et charajump** en le mettant dans le personnage. Ces script est ce qui permet à tous les persos de pouvoir se tabasser entre eux. Ils ne doivent pas être renommés ou modifiés. Ils sont utilisables tel qu'ils sont et les modifier risque de casser les autres persos.

Le scrips d'attaque est le seul à faire pour à chaque personnage, car ils sont susceptibles d'être modifiés si le perso a des attaques différentes de celle de Bobby (no way !).

**Animations**

Commencer par faire les animations, de préférence en pixel art, avec le personnage au milieu de change frame (si vous ne voulez pas passer 1000000 ans à offset l'animation pour que tout colle.

Pour l'instant, de par le kit implémenté, les animations à faire sont:
- Idle
- Course
- Baissé
- Chute
- Saut
- Tilt
- Down Tilt
- Up Tilt
- Neutral Air
- Up Special
- Side Special
- Down Special
- Neutral Special
- Grab
- Shield
- Hit
- Grabbed
- Pummel

Exporter les animation sous un format où toutes les frames sont sur le même .PNG
Une fois les animations prête (les animation clip utilisables), simplement les remplacer dans l'animator.
En cas d'ajout d'animation de windup, remplacer le windup par l'animation de base, désactiver le loop se cette animation puis faire une transistion entre l'animation de windup et l'animation d'attaque. Il faut ensuite faire une transition entre l'animation d'attaque et l'animation de idle.

**Implémentation**

*Important !*
Ne faites pas votre personnage sur Map1 ! Pour être sûr que vous puissiez travailler dessus sans problème, copiez-coller la scène "Workshop: Bobby", renomez la copie et travaillez dessus ! Cela permet que chaque personne puisse travailler sur son perso en même temps et quand même push. Si tout le monde travaille sur la même scène, ça va faire qu'une personne ne pourra pas tester si une autre personne a une erreur !

Le personnage qu'il faut prendre comme "base" est Bobby. Il est conseillé de copier coller le GameObject "Bobby" ainsi que le dossier Bobbyscripts et de changer le nom du dossier et des scripts comme décrit plus tôt. Il ne suffit après plus qu'à remplacer les animations, changer les hitbox et bidouiller les scripts pour en faire votre perso plutôt que de tout recommencer de 0.

Lorsque vous changez les animations, faites attention aux transitions entre animations, aux durées de sorties et aux requirements sinon ça va pas bien marcher. Pour des animations qui ont une animation de windup avant la vrai animation, regarder le up b de Bobby. Dans ce cas précis, il faut utiliser un bool plutôt qu'un trigger pour l'animator.

Essaye d'organiser un minimum le projet, c'est à dire mettre les sprites dans un dossier au nom du personnage dans le dossier My sprites et de faire la même chose avec le dossier scripts.

**Attaques complexes**

Les attaques qui fonctionnent réellement ne sont pas simplement que des attaques qui sont actives pendant frame 1 et ensuite plus jamais. Pour avoir des exemples des attaques plus complexes, merci de se référer à certaines attaques particulière du kit de Bobby à savoir son Nair pour les multi-hit et son down tilt ou son up tilt pour une attaque qui n'est pas active frame one et dont la hitbox reste active pendant quelques frames.

Pour faire une attaque avec plusieurs hitbox, ce qui permet que la hitbox finales soit plus précise qu'une sphère ou qu'un oval, ajouter des transform nomattackpoint (ex: tiltattackpoint). Il ne suffira alors que d'ajouter un nouveau segment qui commence avec un If et avec le compteur de durée d'attaque et boom c'est dans la poche. Il est également possible de séparer les hitbox de cette manière, ce qui rend un tipper possible. Pour plus de précision, voir le Down B de Bobby

Pour faire une attaque avec des projectiles, prendre exemple sur le neutral special de Bobby. Le script "proj" de ses projectiles est à utiliser tel qu'il. Si vous voulez que les projectiles soient chargeables, cela ne pose pas de problème avec mon script étant donné que les modifications se passeront du côté du scirpt d'attaque.

Je peux le dire, ça y est. Mon Bobby n'est pas encore un `Kazuya Mishima` mais il dispose de tous les archétypes d'attaques nécessaire pour servir d'exemple, vous avez donc toutes les clés à disposition (sauf si j'ai oublié des trucs). En attendant, si c'est le cas, faites-le vous même ! Askip on est jamais mieux servi que de ctte façon là.

## Crédits

Projet entamé par Arnaud "Riki" KRAFFT, trésorier de TéléGameDesign 2023/2024

Autres participants:
- Maxime "Lyvent" SOLDATOV, président de TéléGameDesign 2023/2024 (ben oui il a créé le projet, hein)
- personne d'autre pour l'instant, les enfants !