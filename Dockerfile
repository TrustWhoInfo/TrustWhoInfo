FROM mcr.microsoft.com/dotnet/core/sdk:6.0

VOLUME /app
VOLUME /secrets
VOLUME /logs
VOLUME /media
VOLUME /mail

EXPOSE 5000
EXPOSE 5001

RUN apt-get update && apt-get install -y libgdiplus
RUN echo dotnet worldgame.dll \${Node} \> /logs/worldgame-\${Node}.log > /root/run.sh
RUN chmod +x /root/run.sh

ENTRYPOINT cd /app && /root/run.sh