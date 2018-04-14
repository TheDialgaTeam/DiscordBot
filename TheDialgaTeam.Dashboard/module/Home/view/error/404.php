<?php $DevelopmentMode = true; ?>

<h1>A 404 error occurred</h1>
<h2><?= $this->message ?></h2>

<?php
if (isset($this->reason) && $this->reason):
	$reasonMessage= '';
	switch ($this->reason)
	{
		case 'error-controller-cannot-dispatch':
			$reasonMessage = 'The requested controller was unable to dispatch the request.';
			break;
		case 'error-controller-not-found':
			$reasonMessage = 'The requested controller could not be mapped to an existing controller class.';
			break;
		case 'error-controller-invalid':
			$reasonMessage = 'The requested controller was not dispatchable.';
			break;
		case 'error-router-no-match':
			$reasonMessage = 'The requested URL could not be matched by routing.';
			break;
		default:
			$reasonMessage = 'We cannot determine at this time why a 404 was generated.';
			break;
	}
?>
	<p><?= $reasonMessage ?></p>
<?php
endif;

if (isset($this->controller) && $this->controller):
?>
	<dl>
		<dt>Controller:</dt>
		<dd>
			<?= $this->escapeHtml($this->controller) ?>
<?php
			if (isset($this->controller_class) && $this->controller_class && $this->controller_class != $this->controller)
			{
				echo '(' . sprintf('resolves to %s', $this->escapeHtml($this->controller_class)) . ')';
			}
?>
		</dd>
	</dl>
<?php
endif;

if (isset($this->display_exceptions) && $this->display_exceptions):
	if(isset($this->exception) && ($this->exception instanceof Exception || $this->exception instanceof Error)):
?>
		<hr />
		<h2>Additional information:</h2>
		<h3><?= get_class($this->exception) ?></h3>
		<dl>
<?php
		if ($DevelopmentMode):
?>
			<dt>File:</dt>
			<dd>
				<pre class="prettyprint linenums"><?= $this->exception->getFile() ?>:<?= $this->exception->getLine() ?></pre>
			</dd>
			<dt>Message:</dt>
			<dd>
				<pre class="prettyprint linenums"><?= $this->exception->getMessage() ?></pre>
			</dd>
			<dt>Stack trace:</dt>
			<dd>
				<pre class="prettyprint linenums"><?= $this->exception->getTraceAsString() ?></pre>
			</dd>
<?php
		else:
?>
			<dt>Message:</dt>
			<dd>
				<pre class="prettyprint linenums"><?= $this->exception->getMessage() ?></pre>
			</dd>
<?php
		endif;
?>
		</dl>
<?php
		$e = $this->exception->getPrevious();
		$icount = 0;
		if ($e) :
?>
			<hr />
			<h2>Previous exceptions:</h2>
			<ul class="unstyled">
<?php
			while($e) :
?>
				<li>
					<h3><?= get_class($e) ?></h3>
					<dl>
<?php
				if ($DevelopmentMode):
?>
						<dt>File:</dt>
						<dd>
							<pre class="prettyprint linenums"><?= $e->getFile() ?>:<?= $e->getLine() ?></pre>
						</dd>
						<dt>Message:</dt>
						<dd>
							<pre class="prettyprint linenums"><?= $e->getMessage() ?></pre>
						</dd>
						<dt>Stack trace:</dt>
						<dd>
							<pre class="prettyprint linenums"><?= $e->getTraceAsString() ?></pre>
						</dd>
<?php
				else:
?>
						<dt>Message:</dt>
						<dd>
							<pre class="prettyprint linenums"><?= $e->getMessage() ?></pre>
						</dd>
<?php
				endif;						
?>
					</dl>
				</li>
<?php
				$e = $e->getPrevious();
				$icount += 1;
				if ($icount >=50)
				{
					echo "<li>There may be more exceptions, but we have no enough memory to proccess it.</li>";
					break;
				}
			endwhile;
?>
			</ul>
<?php
		endif;
		else:
?>
		<h3>No Exception available</h3>
<?php
	endif;
endif
?>