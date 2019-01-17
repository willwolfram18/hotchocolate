FROM ubuntu:18.04 AS Base

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update \
  && apt-get install apt-utils -y \
  && apt-get install gnupg2 -y \
  && apt-get install apt-transport-https -y \
  && apt-get install wget -y

FROM Base AS GIT

RUN apt-get update \
  && apt-get install git -y

FROM Base AS Java

RUN apt-get update \
  && apt-get install default-jdk -y

FROM Java AS Mono

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
  && echo "deb https://download.mono-project.com/repo/ubuntu vs-bionic main" > /etc/apt/sources.list.d/mono-official.list \
  && apt-get update \
  && apt-get install -y mono-complete \
  && rm -rf /var/lib/apt/lists/* /tmp/*

FROM Mono AS NetCore

RUN wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb \
  && dpkg -i packages-microsoft-prod.deb \
  && apt-get update \
  && apt-get install dotnet-sdk-2.1 -y \
  && apt-get install dotnet-sdk-2.2 -y

FROM NetCore AS Tools

RUN apt-get update \
  && apt-get install -y binutils curl ca-certificates-mono fsharp mono-vbnc nuget referenceassemblies-pcl \
  && dotnet tool install -g Cake.Tool --version 0.32.1

ENV PATH="$PATH:/root/.dotnet/tools"

FROM Tools AS Builder

COPY ./ /hc
WORKDIR /hc

RUN dotnet cake -target=CoreTests
