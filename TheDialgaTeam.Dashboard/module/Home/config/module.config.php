<?php
namespace Home
{
	use Zend\Router\Http\Literal;
	use Zend\Router\Http\Segment;
	use Zend\ServiceManager\Factory\InvokableFactory;

	return [
			'router' => [
				'routes' => [
					'home' => [
						'type' => Literal::class,
						'options' => [
							'route'    => '/',
							'defaults' => [
								'controller' => Controller\IndexController::class,
								'action' => 'index',
							],
						],
					],
                    'login' => [
                        'type' => Literal::class,
                        'options' => [
                            'route'    => '/login',
                            'defaults' => [
                                'controller' => Controller\IndexController::class,
                                'action' => 'login',
                            ],
                        ],
                    ],
				],
			],
			'controllers' => [
				'factories' => [
					Controller\IndexController::class => InvokableFactory::class,
				],
			],
			'view_manager' => [
				'display_not_found_reason' => true,
				'display_exceptions' => true,
				'doctype' => 'HTML5',
				'not_found_template' => 'error/404',
				'exception_template' => 'error/index',
				'default_template_suffix' => 'php',
				'template_map' => [
					'layout/layout' => __DIR__ . '/../view/layout/layout.php',
					'error/404' => __DIR__ . '/../view/error/404.php',
					'error/index' => __DIR__ . '/../view/error/index.php',
				],
				'template_path_stack' => [
					'home' => __DIR__ . '/../view',
				],
			],
		];
}
?>