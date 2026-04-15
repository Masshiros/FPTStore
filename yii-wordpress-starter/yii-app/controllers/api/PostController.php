<?php

namespace app\controllers\api;

use app\models\Post;
use yii\rest\ActiveController;

class PostController extends ActiveController
{
    public $modelClass = Post::class;

    public function behaviors(): array
    {
        $behaviors = parent::behaviors();
        $behaviors['contentNegotiator']['formats']['application/json'] = \yii\web\Response::FORMAT_JSON;
        return $behaviors;
    }

    public function actions(): array
    {
        $actions = parent::actions();
        $actions['index']['prepareDataProvider'] = function () {
            $query = Post::find();
            $status = \Yii::$app->request->get('status');

            if ($status !== null && in_array((int)$status, [0, 1], true)) {
                $query->andWhere(['status' => (int)$status]);
            }

            return new \yii\data\ActiveDataProvider([
                'query' => $query->orderBy(['id' => SORT_DESC]),
                'pagination' => ['pageSize' => 20],
            ]);
        };

        return $actions;
    }
}
