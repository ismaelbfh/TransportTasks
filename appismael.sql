-- phpMyAdmin SQL Dump
-- version 4.7.9
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1:3307
-- Tiempo de generación: 20-03-2018 a las 17:15:33
-- Versión del servidor: 8.0.4-rc-log
-- Versión de PHP: 7.0.25-0ubuntu0.16.04.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

--
-- Base de datos: `agendaismael`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Cliente`
--

CREATE TABLE `Cliente` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(25) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Apellidos` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FechaNacimiento` date NOT NULL,
  `Telefono` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IdUsuario` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `Cliente`
--

INSERT INTO `Cliente` (`Id`, `Nombre`, `Apellidos`, `FechaNacimiento`, `Telefono`, `IdUsuario`) VALUES
(2, 'Cliente Default', 'Default', '2018-03-02', '555444333', 14),
(6, 'n', 'n', '1996-07-12', '777888999', 14),
(8, 'dios', 'dios', '1900-01-09', '666222333', 14),
(9, 'hy', 'hy', '1900-01-09', '777666111', 14),
(10, 'lol', 'lol', '1900-01-14', '111222333', 14),
(11, 'wep', 'wep', '1900-01-05', '222999888', 14),
(14, 'Default client', 'default client', '1900-01-12', '321555666', 19),
(15, 'otro', 'otro', '1900-01-09', '123456980', 19),
(16, 'default', 'def', '1900-01-17', '444555666', 20),
(17, 'other', 'other', '1900-01-09', '666333111', 20),
(18, 'Default Client', 'Client Default', '1990-12-12', '666999000', 21),
(19, 'Default Client', 'Client Default', '1990-12-12', '666999000', 22);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Direccion`
--

CREATE TABLE `Direccion` (
  `Id` int(11) NOT NULL,
  `IdTarea` int(11) NOT NULL,
  `DireccionActual` varchar(80) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Latitud` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Longitud` varchar(10) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Distancia` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IdCliente` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `Direccion`
--

INSERT INTO `Direccion` (`Id`, `IdTarea`, `DireccionActual`, `Latitud`, `Longitud`, `Distancia`, `IdCliente`) VALUES
(55, 66, 'IIOHNJN', '12', '22', '90.00 Km.', 2),
(56, 68, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 2),
(57, 69, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 8),
(58, 69, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91', 2),
(59, 69, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,92', 2),
(63, 72, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 14),
(64, 73, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 15),
(65, 74, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 14),
(66, 75, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 15),
(67, 75, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91', 14),
(68, 75, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91', 15),
(69, 76, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 16),
(70, 77, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 17),
(71, 78, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 16),
(72, 79, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 17),
(73, 78, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91', 17),
(74, 79, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91', 16),
(75, 80, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91 Km.', 10),
(76, 80, 'Calle Pablo Ruiz Picasso, 64, 50018 Zaragoza, España  ', '41,66', '-0,90', '1.092,91', 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Empresa`
--

CREATE TABLE `Empresa` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(35) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Funcion` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Sector` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Ciudad` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Telefono` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `Empresa`
--

INSERT INTO `Empresa` (`Id`, `Nombre`, `Funcion`, `Sector`, `Ciudad`, `Telefono`) VALUES
(2, 'Default Empresa', 'Default', 'Default', 'Default City', '666555444'),
(3, 'Vueling', 'Vuelos', 'Transportes', 'Zaragoza', '666333444'),
(6, 'Brute Force Games', 'videojuegos', 'Videojuegos', 'Huesca', '666444111'),
(7, 'hola', 'hola', 'Judicial', 'hola', '333222111'),
(8, 'ye', 'ye', 'Judicial', 'ye', '111333444'),
(9, 'gjjf', 'gyxuc', 'Comercial', 'yxxic', '332111555');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Tarea`
--

CREATE TABLE `Tarea` (
  `Id` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `NombreTarea` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TipoTarea` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Descripcion` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `HoraInicio` datetime NOT NULL,
  `HoraFin` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `Tarea`
--

INSERT INTO `Tarea` (`Id`, `IdUsuario`, `NombreTarea`, `TipoTarea`, `Descripcion`, `HoraInicio`, `HoraFin`) VALUES
(65, 14, 'ola', 'Deportes', 'ola', '2018-03-19 12:20:07', NULL),
(66, 14, 'ssss', 'Deportes', 'sssss', '2018-03-19 11:31:38', NULL),
(67, 14, 'ef', 'Deportes', 'effee', '2018-03-19 13:36:07', NULL),
(68, 14, 'isa', 'Deportes', 'isa', '2018-03-19 14:08:09', NULL),
(69, 14, 'TareaNueva', 'Deportes', 'Hacenos de todo primero emos echo algo', '2018-03-19 16:26:55', NULL),
(72, 19, 'nuevaClient', 'Deportes', 'aqui', '2018-03-20 15:14:33', NULL),
(73, 19, 'nueva2', 'Deportes', 'nueva2', '2018-03-20 15:15:15', NULL),
(74, 19, 'de ', 'Juegos', 'de', '2018-03-20 15:15:33', '2018-03-20 15:16:50'),
(75, 19, 'de2', 'Escolar', 'de2', '2018-03-20 15:15:53', '2018-03-20 15:16:54'),
(76, 20, 'ge', 'Deportes', 'te', '2018-03-20 15:20:02', NULL),
(77, 20, 'lo', 'Informatica', 'pe', '2018-03-20 15:20:20', NULL),
(78, 20, 'ni', 'Television', 'to', '2018-03-20 15:20:40', '2018-03-20 15:21:14'),
(79, 20, 'uo', 'Juegos', 'ytw', '2018-03-20 15:21:01', '2018-03-20 15:21:32'),
(80, 14, 'lo', 'Deportes', 'loca', '2018-03-20 16:46:39', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `Usuario`
--

CREATE TABLE `Usuario` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Profesion` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Telefono` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Nick` varchar(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Password` varchar(25) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Rol` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL,
  `IdEmpresa` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `Usuario`
--

INSERT INTO `Usuario` (`Id`, `Nombre`, `Profesion`, `Telefono`, `Nick`, `Password`, `Rol`, `IdEmpresa`) VALUES
(14, 'admin', 'Super Administrador', '666444333', 'admin', 'admin', 'SuperAdmin', 2),
(19, 'ola', 'ola', '222111555', 'ola', 'ola', 'Usuario', 3),
(20, 'adios', 'adios', '123456789', 'adios', 'adios', 'Usuario', 7),
(21, 'lol', 'lol', '123456789', 'lol', 'lol', 'Usuario', 6),
(22, 'jefe', 'jefe', '548454646', 'jefe', 'jefe', 'Admin', 6);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `Cliente`
--
ALTER TABLE `Cliente`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdUsuario` (`IdUsuario`);

--
-- Indices de la tabla `Direccion`
--
ALTER TABLE `Direccion`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdTarea` (`IdTarea`),
  ADD KEY `IdCliente` (`IdCliente`);

--
-- Indices de la tabla `Empresa`
--
ALTER TABLE `Empresa`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `Tarea`
--
ALTER TABLE `Tarea`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdUsuario` (`IdUsuario`);

--
-- Indices de la tabla `Usuario`
--
ALTER TABLE `Usuario`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdEmpresa` (`IdEmpresa`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `Cliente`
--
ALTER TABLE `Cliente`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT de la tabla `Direccion`
--
ALTER TABLE `Direccion`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=77;

--
-- AUTO_INCREMENT de la tabla `Empresa`
--
ALTER TABLE `Empresa`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `Tarea`
--
ALTER TABLE `Tarea`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=81;

--
-- AUTO_INCREMENT de la tabla `Usuario`
--
ALTER TABLE `Usuario`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `Cliente`
--
ALTER TABLE `Cliente`
  ADD CONSTRAINT `Cliente_ibfk_1` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `Direccion`
--
ALTER TABLE `Direccion`
  ADD CONSTRAINT `Direccion_ibfk_1` FOREIGN KEY (`IdTarea`) REFERENCES `Tarea` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `Direccion_ibfk_2` FOREIGN KEY (`IdCliente`) REFERENCES `Cliente` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `Tarea`
--
ALTER TABLE `Tarea`
  ADD CONSTRAINT `Tarea_ibfk_1` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario` (`id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `Usuario`
--
ALTER TABLE `Usuario`
  ADD CONSTRAINT `Usuario_ibfk_1` FOREIGN KEY (`IdEmpresa`) REFERENCES `Empresa` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
