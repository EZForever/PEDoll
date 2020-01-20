#include <iostream>
#include <exception>
#include <stdexcept>

#include "../libPuppet/libPuppet.h"
#include "../libPuppet/PuppetClientTCP.h"

int main()
{
    Puppet::IPuppet* client = NULL;

    try
    {
        client = new Puppet::PuppetClientTCP(8888);
        client->start();
        
        Puppet::PACKET_ACK packetOut;
        Puppet::PACKET* packetIn = NULL;
        do
        {
            try
            {
                packetIn = client->recv();
                if (packetIn->type == Puppet::PACKET_TYPE::CMD_ANY)
                    client->send(packetOut);
                delete packetIn;
            }
            catch (const std::runtime_error& e)
            {
                std::cerr << e.what() << std::endl;
                packetIn = NULL;
            }
        }
        while (packetIn);
        delete client;
    }
    catch (const std::runtime_error& e)
    {
        std::cerr << e.what() << std::endl;
        std::cerr << "lastError = " << client->lastError << std::endl;
    }
    return 0;
}

