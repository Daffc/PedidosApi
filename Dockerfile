FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /workspace

ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet tool install --global dotnet-ef
RUN dotnet tool install --global dotnet-reportgenerator-globaltool

CMD ["sleep", "infinity"]