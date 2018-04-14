<?php

namespace Home\Model\Discord;

class OAuth2
{
    const AUTHORIZEURL = 'https://discordapp.com/api/oauth2/authorize';
    const TOKENURL = 'https://discordapp.com/api/oauth2/token';
    const TOKENREVOKURL = 'https://discordapp.com/api/oauth2/token/revoke';

    public function createAuthorizationUrl($clientId, $scope, $state)
    {
    }
}