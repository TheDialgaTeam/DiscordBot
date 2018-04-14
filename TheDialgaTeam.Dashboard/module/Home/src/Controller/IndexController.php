<?php
namespace Home\Controller
{
    use Zend\Http\Client;
    use Zend\Http\Request;
    use Zend\Json\Json;
    use Zend\Mvc\Controller\AbstractActionController;
    use Zend\View\Model\ViewModel;

    class IndexController extends AbstractActionController
    {
        public function indexAction()
        {
            $request = (new Request())
                ->setUri('http://127.0.0.1:5000/discordAppModel')
                ->setMethod('GET');

            $response = (new Client())->send($request);

            $jsonObject = Json::decode($response->getBody(), Json::TYPE_ARRAY);

            return new ViewModel([ json => $jsonObject ]);
        }

        public function loginAction()
        {
            return new ViewModel([ requestHeader => $_POST['clientId'] ]);
        }
    }
}