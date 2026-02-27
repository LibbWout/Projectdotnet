SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

CREATE TABLE `rollen` (
  `id` int(11) NOT NULL,
  `naam` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `gebruikers` (
  `id` int(11) NOT NULL,
  `naam` varchar(255) NOT NULL,
  `wachtwoord_hash` char(255) NOT NULL,
  `unieke_code` varchar(255) NOT NULL,
  `tijdstip_geactiveerd` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `roltoewijzingen` (
  `gebruiker_id` int(11) NOT NULL,
  `rol_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `tafels` (
  `id` int(11) NOT NULL,
  `naam` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `tafeltoewijzingen` (
  `gebruiker_id` int(11) NOT NULL,
  `tafel_id` int(11) NOT NULL,
  `tijdstip_toegewezen` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `producten` (
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `productdetails` (
  `product_id` int(11) NOT NULL,
  `tijdstip` datetime NOT NULL,
  `naam` varchar(255) NOT NULL,
  `prijs` float NOT NULL,
  `producttype` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `bestellingen` (
  `id` int(11) NOT NULL,
  `gebruiker_id` int(11) NOT NULL,
  `tijdstip_besteld` datetime NOT NULL,
  `status` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

CREATE TABLE `bestellijnen` (
  `bestelling_id` int(11) NOT NULL,
  `product_id` int(11) NOT NULL,
  `hoeveelheid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;


ALTER TABLE `rollen`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `gebruikers`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `roltoewijzingen`
  ADD PRIMARY KEY (`gebruiker_id`, `rol_id`),
  ADD KEY `fk__roltoewijzingen__rol_id` (`rol_id`);

ALTER TABLE `tafels`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `tafeltoewijzingen`
  ADD PRIMARY KEY (`gebruiker_id`, `tafel_id`, `tijdstip_toegewezen`),
  ADD KEY `fk__tafeltoewijzingen__tafel_id` (`tafel_id`);

ALTER TABLE `producten`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `productdetails`
  ADD PRIMARY KEY (`product_id`, `tijdstip`),
  ADD KEY `fk__productdetails__product_id` (`product_id`);

ALTER TABLE `bestellingen`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk__bestellingen__gebruiker_id` (`gebruiker_id`);

ALTER TABLE `bestellijnen`
  ADD PRIMARY KEY (`bestelling_id`, `product_id`),
  ADD KEY `fk__bestellijnen__product_id` (`product_id`);


ALTER TABLE `rollen`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `gebruikers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `tafels`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `producten`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `bestellingen`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;


ALTER TABLE `roltoewijzingen`
  ADD CONSTRAINT `fk__roltoewijzingen__gebruiker_id` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`),
  ADD CONSTRAINT `fk__roltoewijzingen__rol_id` FOREIGN KEY (`rol_id`) REFERENCES `rollen` (`id`);

ALTER TABLE `tafeltoewijzingen`
  ADD CONSTRAINT `fk__tafeltoewijzingen__gebruiker_id` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`),
  ADD CONSTRAINT `fk__tafeltoewijzingen__tafel_id` FOREIGN KEY (`tafel_id`) REFERENCES `tafels` (`id`);

ALTER TABLE `productdetails`
  ADD CONSTRAINT `fk__productdetails__product_id` FOREIGN KEY (`product_id`) REFERENCES `producten` (`id`);

ALTER TABLE `bestellingen`
  ADD CONSTRAINT `fk__bestellingen__gebruiker_id` FOREIGN KEY (`gebruiker_id`) REFERENCES `gebruikers` (`id`);

ALTER TABLE `bestellijnen`
  ADD CONSTRAINT `fk__bestellijnen__bestelling_id` FOREIGN KEY (`bestelling_id`) REFERENCES `bestellingen` (`id`),
  ADD CONSTRAINT `fk__bestellijnen__product_id` FOREIGN KEY (`product_id`) REFERENCES `producten` (`id`);

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;