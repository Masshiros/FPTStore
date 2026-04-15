<?php

use yii\grid\ActionColumn;
use yii\grid\GridView;
use yii\helpers\Html;

/** @var yii\web\View $this */
/** @var app\models\PostSearch $searchModel */
/** @var yii\data\ActiveDataProvider $dataProvider */

$this->title = 'Posts';
$this->params['breadcrumbs'][] = $this->title;
?>
<div class="post-index">

    <h1><?= Html::encode($this->title) ?></h1>

    <p>
        <?= Html::a('Create Post', ['create'], ['class' => 'btn btn-success']) ?>
    </p>

    <?= GridView::widget([
        'dataProvider' => $dataProvider,
        'filterModel' => $searchModel,
        'columns' => [
            'id',
            'title',
            [
                'attribute' => 'status',
                'filter' => [1 => 'Published', 0 => 'Draft'],
                'value' => static fn ($model) => $model->status ? 'Published' : 'Draft',
            ],
            'created_at',
            [
                'class' => ActionColumn::class,
                'urlCreator' => static fn ($action, $model) => [$action, 'id' => $model->id],
            ],
        ],
    ]); ?>

</div>
