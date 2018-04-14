<?= $this->doctype() ?>

<html lang="en">
<head>
	<meta charset="utf-8" />
	<?= $this->headTitle('The Dialga Team Bot Web Dashboard')->setSeparator(' - ')->setAutoEscape(false) ?>

	<?= $this->headMeta()
		->appendName('viewport', 'width=device-width, initial-scale=1.0')
		->appendHttpEquiv('X-UA-Compatible', 'IE=edge')
    ?>

	<!-- Le styles -->
	<?= $this->headLink(['rel' => 'shortcut icon', 'type' => 'image/vnd.microsoft.icon', 'href' => $this->basePath() . '/img/favicon.ico'])
		->prependStylesheet($this->basePath('css/extra.css'))
		->prependStylesheet($this->basePath('css/style.css'))
		->prependStylesheet($this->basePath('vendor/twbs/bootstrap/dist/css/bootstrap.min.css'))
		->prependStylesheet($this->basePath('vendor/twbs/bootstrap/dist/css/bootstrap-theme.min.css'))
	?>

	<!-- Scripts -->
	<?= $this->headScript()
		->prependFile($this->basePath('vendor/twbs/bootstrap/dist/js/bootstrap.min.js'))
		->prependFile($this->basePath('vendor/components/jquery/jquery.min.js'))
    ?>
</head>
<body>
    <nav class="navbar navbar-expand-md navbar-dark fixed-top" style="background-color: darkorange;">
        <a class="navbar-brand" style="color: black" href="<?= $this->url('home') ?>">
            <img src="<?= $this->basePath('img/logo.png') ?>" height="25" />
            The Dialga Team Bot
        </a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExampleDefault" aria-controls="navbarsExampleDefault" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarsExampleDefault">
            <ul class="navbar-nav mr-auto">
                <li class="nav-item active">
                    <a class="nav-link" style="color: black" href="<?= $this->url('home') ?>">Home</a>
                </li>
                <li class="nav-item active">
                    <a class="nav-link" style="color: black" href="#">Test</a>
                </li>
            </ul>
            <a class="btn btn-primary" role="button" style="color: white">Login with Discord</a>
        </div>
    </nav>
	<div class="container">
		<?= $this->content ?>
		<hr />
		<footer>
			<p>
				This site is powered by
				<a href="https://framework.zend.com/">Zend Framework</a>.
				<br />
				&copy; 2015 - <?= date('Y') ?> by The Dialga Team.
			</p>
		</footer>
	</div>
	<?= $this->inlineScript() ?>
</body>
</html>
