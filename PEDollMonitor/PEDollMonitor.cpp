#include <iostream>
#include <exception>
#include <stdexcept>

#include "../libPuppetProtocol/libPuppetProtocol.h"
#include "../libPuppetProtocol/PuppetClientTCP.h"

int main()
{
    try
    {
        Puppet::IPuppetClient* client = new Puppet::PuppetClientTCP(8888);
        client->connect();
        
        Puppet::PACKET_MSG_ACK packetOut;
        Puppet::PACKET* packetIn = NULL;
        do
        {
            try
            {
                packetIn = client->recv();
                if (packetIn->type == Puppet::PACKET_TYPE::CMD_PING)
                    client->send(packetOut);
                delete packetIn;
            }
            catch (const std::runtime_error& e)
            {
                packetIn = NULL;
            }
        }
        while (packetIn);
        delete client;
    }
    catch (const std::runtime_error& e)
    {
        std::cerr << e.what() << std::endl;
        std::cerr << "Puppet::lastError = " << Puppet::lastError << std::endl;
    }
    return 0;
}

